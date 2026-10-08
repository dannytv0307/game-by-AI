"""Tổng hợp SFX retro bằng code (không gọi API). Seed cố định => kết quả tái lập được."""
import numpy as np

SR = 44100


def _t(dur):
    return np.arange(int(SR * dur)) / SR


def _env(n, attack=0.005, decay_pow=2.0):
    a = max(1, int(SR * attack))
    e = np.ones(n)
    e[:a] = np.linspace(0, 1, a)
    e[a:] = np.linspace(1, 0, n - a) ** decay_pow
    return e


def _sweep(f0, f1, dur, wave="sine"):
    t = _t(dur)
    f = f0 * (f1 / f0) ** (t / dur)  # quét theo hàm mũ
    ph = 2 * np.pi * np.cumsum(f) / SR
    if wave == "square":
        return np.sign(np.sin(ph)) * 0.6
    if wave == "saw":
        return 2 * ((ph / (2 * np.pi)) % 1) - 1
    return np.sin(ph)


def _lowpass(x, cutoff):
    """Lọc thông thấp 1 cực; cutoff có thể là mảng (lọc quét)."""
    cutoff = np.broadcast_to(cutoff, x.shape)
    a = 1 - np.exp(-2 * np.pi * cutoff / SR)
    y = np.empty_like(x)
    acc = 0.0
    for i in range(len(x)):
        acc += a[i] * (x[i] - acc)
        y[i] = acc
    return y


def _noise(dur, rng):
    return rng.uniform(-1, 1, int(SR * dur))


def _crush(x, bits=6):
    q = 2 ** bits
    return np.round(x * q) / q


def slash(rng):
    d = 0.2
    n = _noise(d, rng)
    x = _lowpass(n, np.linspace(9000, 1200, len(n))) - _lowpass(n, 400)
    return x * _env(len(x), 0.004, 1.6) * 1.4


def hit(rng):
    d = 0.18
    body = _sweep(140, 45, d) * _env(int(SR * d), 0.002, 2.5)
    crack = _lowpass(_noise(d, rng), 3000) * _env(int(SR * d), 0.001, 6)
    return np.tanh((body + crack * 0.8) * 2)


def jump(rng):
    d = 0.16
    return _crush(_sweep(260, 720, d, "square")) * _env(int(SR * d), 0.003, 1.2)


def land(rng):
    d = 0.12
    thump = _sweep(110, 50, d) * _env(int(SR * d), 0.002, 3)
    dust = _lowpass(_noise(d, rng), 900) * _env(int(SR * d), 0.002, 2)
    return thump + dust * 0.6


def hurt(rng):
    d = 0.28
    t = _t(d)
    x = _sweep(620, 180, d, "square") * (1 + 0.3 * np.sin(2 * np.pi * 28 * t))
    return _crush(x * 0.7) * _env(len(t), 0.003, 1.5)


def rage_full(rng):
    notes = [220, 277.2, 329.6, 440, 554.4, 659.3]
    seg = 0.07
    x = np.concatenate([_sweep(f, f * 1.01, seg, "square") * _env(int(SR * seg), 0.002, 1) for f in notes])
    tail = _sweep(880, 1320, 0.35) * _env(int(SR * 0.35), 0.01, 2) * 0.5
    x = np.concatenate([x, tail])
    shimmer = _lowpass(_noise(len(x) / SR, rng), 6000) * np.linspace(0, 0.25, len(x))
    return _crush(x * 0.8) + shimmer


def transform(rng):
    d = 1.6
    n = int(SR * d)
    rumble = _sweep(35, 90, d, "saw") * np.linspace(0.2, 1, n) ** 2
    rumble = _lowpass(rumble, 300)
    swell = _lowpass(_noise(d, rng), np.linspace(300, 7000, n)) * np.linspace(0, 1, n) ** 3
    scream = _sweep(200, 900, d, "saw") * np.linspace(0, 0.6, n) ** 2
    x = rumble * 1.2 + swell * 0.7 + _lowpass(scream, 2500)
    burst = hit(rng) * 1.2
    x = np.concatenate([x, np.zeros(int(SR * 0.3))])
    x[n - 200:n - 200 + len(burst)] += burst
    return np.tanh(x * 1.6)


def enemy_die(rng):
    d = 0.45
    crunch = _crush(_lowpass(_noise(d, rng), 2500), 4) * _env(int(SR * d), 0.002, 2)
    fall = _crush(_sweep(500, 60, d, "square")) * _env(int(SR * d), 0.002, 1.3)
    return crunch * 0.7 + fall * 0.6


PRESETS = {f.__name__: f for f in (slash, hit, jump, land, hurt, rage_full, transform, enemy_die)}


def render(preset, seed):
    if preset not in PRESETS:
        raise SystemExit(f"Preset SFX '{preset}' không tồn tại. Có: {', '.join(PRESETS)}")
    x = PRESETS[preset](np.random.default_rng(seed))
    return x[:, None], SR
