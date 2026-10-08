"""Xử lý âm thanh: đọc/ghi WAV, chuẩn hóa độ lớn, tạo vòng lặp liền mạch, hiệu ứng giọng quỷ."""
import io
import wave
from pathlib import Path

import numpy as np


def read_wav_bytes(data):
    with wave.open(io.BytesIO(data)) as w:
        sr, ch, sw = w.getframerate(), w.getnchannels(), w.getsampwidth()
        raw = w.readframes(w.getnframes())
    if sw != 2:
        raise RuntimeError(f"Chỉ hỗ trợ WAV 16-bit, nhận {sw * 8}-bit")
    x = np.frombuffer(raw, dtype="<i2").astype(np.float64) / 32768.0
    return x.reshape(-1, ch), sr


def pcm16_to_array(pcm):
    return (np.frombuffer(pcm, dtype="<i2").astype(np.float64) / 32768.0).reshape(-1, 1)


def write_wav(path, x, sr):
    path = Path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    x = np.atleast_2d(x.T).T if x.ndim == 1 else x
    data = (np.clip(x, -1, 1) * 32767).astype("<i2")
    with wave.open(str(path), "wb") as w:
        w.setnchannels(data.shape[1])
        w.setsampwidth(2)
        w.setframerate(sr)
        w.writeframes(data.tobytes())


def db(v):
    return 20 * np.log10(max(v, 1e-9))


def normalize_rms(x, target_db, peak_db=-1.0):
    """Đưa RMS về target_db, sau đó giới hạn đỉnh mềm để không vượt peak_db."""
    rms = np.sqrt(np.mean(x ** 2))
    y = x * 10 ** ((target_db - db(rms)) / 20)
    ceiling = 10 ** (peak_db / 20)
    return np.tanh(y / ceiling) * ceiling if np.abs(y).max() > ceiling else y


def normalize_peak(x, peak_db):
    return x * (10 ** (peak_db / 20) / max(np.abs(x).max(), 1e-9))


def make_loop(x, sr, xfade_s=2.0):
    """Trộn chéo đuôi vào đầu để track lặp liền mạch."""
    n = int(sr * xfade_s)
    if len(x) < n * 3:
        return x
    fade = np.linspace(0, 1, n)[:, None]
    head, tail = x[:n], x[-n:]
    out = x[:-n].copy()
    out[:n] = head * np.sqrt(fade) + tail * np.sqrt(1 - fade)
    return out


def trim_silence(x, sr, thresh_db=-45, pad_s=0.05):
    env = np.abs(x).max(1)
    idx = np.nonzero(env > 10 ** (thresh_db / 20))[0]
    if len(idx) == 0:
        return x
    p = int(sr * pad_s)
    return x[max(0, idx[0] - p): idx[-1] + p]


def _resample(x, factor):
    """Đổi tốc độ phát (factor<1 = chậm & trầm hơn)."""
    n = int(len(x) / factor)
    src = np.linspace(0, len(x) - 1, n)
    return np.stack([np.interp(src, np.arange(len(x)), x[:, c]) for c in range(x.shape[1])], 1)


def demon_fx(x, sr):
    """Giọng quỷ: hạ tông, chồng lớp quãng tám thấp, méo nhẹ, vang ngắn."""
    low = _resample(x, 0.85)
    oct_ = _resample(x, 0.5)[: len(low)]
    oct_ = np.pad(oct_, ((0, len(low) - len(oct_)), (0, 0)))
    y = low + 0.45 * oct_
    y = np.tanh(y * 2.2) / 2.2
    out = np.pad(y, ((0, int(sr * 0.4)), (0, 0)))
    for delay, g in ((0.045, 0.35), (0.11, 0.22), (0.23, 0.12)):
        d = int(sr * delay)
        out[d:d + len(y)] += g * y
    return out
