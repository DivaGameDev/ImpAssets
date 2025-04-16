#v5 with animated subtitles
import whisper
import warnings
import ffmpeg
import os

warnings.filterwarnings("ignore", message="FP16 is not supported on CPU; using FP32 instead")

def transcribe_audio(audio_path):
    model = whisper.load_model("base", device="cpu")
    result = model.transcribe(audio_path, word_timestamps=True)
    return result['segments']

def create_ass(segments, ass_file):
    with open(ass_file, 'w', encoding='utf-8') as f:
        f.write("[Script Info]\n")
        f.write("Title: Pop-up Subtitles\n")
        f.write("[V4+ Styles]\n")
        f.write("Format: Name, Fontname, Fontsize, PrimaryColour, OutlineColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding\n")
        f.write("Style: PopText,Montserrat Bold,28,&H00FFFF&,0,0,-1,0,1,1,2,2,20,20,60\n")  
        # ↑ Alignment 2 = Bottom-center, MarginV ensures better spacing

        f.write("[Events]\n")
        f.write("Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text\n")

        for idx, segment in enumerate(segments):
            for word_info in segment['words']:
                start = word_info['start']
                end = word_info['end']
                text = word_info['word'].replace('\n', '\\N')

                start_time = f"{int(start // 3600):02}:{int((start % 3600) // 60):02}:{start % 60:.2f}"
                end_time = f"{int(end // 3600):02}:{int((end % 3600) // 60):02}:{end % 60:.2f}"

                # Pop-in effect with a fade-in (200ms) and fade-out (100ms)
                f.write(f"Dialogue: 0,{start_time},{end_time},PopText,,0,0,30,,{{\\fad(200,100)}}{text}\n")

def embed_subtitles(video_path, ass_file, output_path):
    safe_ass_path = ass_file.replace('\\', '/').replace(':', '\\:')

    (
        ffmpeg
        .input(video_path)
        .output(
            output_path,
            vf=f"ass='{safe_ass_path}'",
            vcodec='libx264',
            acodec='aac',
            strict='-2'
        )
        .run()
    )

# Example Usage
audio_path = "e:/Editings/recordings/3 31 25.mp3"
video_path = "e:/Editings/recordings/3 31 25.mp4"
ass_file = "E:/Editings/recordings/subtitles.ass"
output_path = "E:/Editings/recordings/output.mp4"

if not os.path.exists(audio_path) or not os.path.exists(video_path):
    print("❌ Error: Audio or video file not found. Check the file paths.")
else:
    segments = transcribe_audio(audio_path)
    create_ass(segments, ass_file)

    user_input = input("Subtitles generated. Do you want to embed them in the video? (yes/no): ").strip().lower()
    if user_input == 'yes':
        embed_subtitles(video_path, ass_file, output_path)
        print(f"✅ Animated pop-up subtitles added successfully to {output_path}")
    else:
        print("❌ Subtitle embedding canceled.")

