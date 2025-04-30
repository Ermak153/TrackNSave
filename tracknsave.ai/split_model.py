import os

def split_file(file_path, chunk_size=1500 * 1024 * 1024):
    part_num = 0
    with open(file_path, 'rb') as f:
        while True:
            chunk = f.read(chunk_size)
            if not chunk:
                break
            part_path = f"{file_path}_part_{part_num:02d}"
            with open(part_path, 'wb') as part_file:
                part_file.write(chunk)
            part_num += 1

split_file("models/xlm-roberta-large/model.safetensors")