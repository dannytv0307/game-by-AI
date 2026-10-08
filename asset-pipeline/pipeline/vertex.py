"""Gọi Vertex AI: Gemini image, Gemini TTS, Lyria."""
import base64

import google.auth
import google.auth.transport.requests
import requests
from google import genai
from google.genai import types

_clients = {}


def _client(project, location):
    k = (project, location)
    if k not in _clients:
        _clients[k] = genai.Client(vertexai=True, project=project, location=location)
    return _clients[k]


def generate_image(cfg, model, prompt, refs, aspect, image_size=None):
    """refs: list[Path]. image_size: None (mặc định của mô hình) | "1K" | "2K" | "4K". Trả về bytes PNG."""
    c = _client(cfg["gcp"]["project"], cfg["gcp"]["image_location"])
    mimes = {".jpg": "image/jpeg", ".jpeg": "image/jpeg", ".webp": "image/webp"}
    contents = [types.Part.from_bytes(data=p.read_bytes(), mime_type=mimes.get(p.suffix.lower(), "image/png")) for p in refs]
    contents.append(prompt)
    r = c.models.generate_content(
        model=model,
        contents=contents,
        config=types.GenerateContentConfig(
            response_modalities=["IMAGE"],
            image_config=types.ImageConfig(aspect_ratio=aspect, image_size=image_size),
        ),
    )
    for part in r.candidates[0].content.parts:
        if part.inline_data and part.inline_data.mime_type.startswith("image/"):
            return part.inline_data.data
    text = " ".join(p.text for p in r.candidates[0].content.parts if p.text)
    raise RuntimeError(f"Mô hình không trả ảnh. finish_reason={r.candidates[0].finish_reason} text={text[:300]}")


def generate_speech(cfg, model, text, voice):
    """Trả về PCM 16-bit mono 24 kHz."""
    c = _client(cfg["gcp"]["project"], cfg["gcp"]["image_location"])
    r = c.models.generate_content(
        model=model,
        contents=text,
        config=types.GenerateContentConfig(
            response_modalities=["AUDIO"],
            speech_config=types.SpeechConfig(
                voice_config=types.VoiceConfig(prebuilt_voice_config=types.PrebuiltVoiceConfig(voice_name=voice))
            ),
        ),
    )
    return r.candidates[0].content.parts[0].inline_data.data


def judge_audio(cfg, model, wav_bytes, instructions):
    """Cho mô hình đa phương thức nghe file WAV và trả về nhận xét JSON (dùng để chấm ứng viên âm thanh)."""
    c = _client(cfg["gcp"]["project"], cfg["gcp"]["image_location"])
    r = c.models.generate_content(
        model=model,
        contents=[types.Part.from_bytes(data=wav_bytes, mime_type="audio/wav"), instructions],
        config=types.GenerateContentConfig(response_mime_type="application/json", temperature=0),
    )
    return r.text


def generate_music(cfg, model, prompt, negative, seed):
    """Trả về bytes WAV (Lyria: ~30 s, 48 kHz stereo)."""
    creds, _ = google.auth.default(scopes=["https://www.googleapis.com/auth/cloud-platform"])
    creds.refresh(google.auth.transport.requests.Request())
    proj, loc = cfg["gcp"]["project"], cfg["gcp"]["music_location"]
    url = f"https://{loc}-aiplatform.googleapis.com/v1/projects/{proj}/locations/{loc}/publishers/google/models/{model}:predict"
    body = {"instances": [{"prompt": prompt, "negative_prompt": negative, "seed": seed}], "parameters": {}}
    r = requests.post(url, headers={"Authorization": f"Bearer {creds.token}"}, json=body, timeout=600)
    if r.status_code != 200:
        raise RuntimeError(f"Lyria lỗi {r.status_code}: {r.text[:500]}")
    pred = r.json()["predictions"][0]
    return base64.b64decode(pred.get("bytesBase64Encoded") or pred["audioContent"])
