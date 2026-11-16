import os
import math
import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont

output_dir = "Assets/Carpeta_Alejo/Office_Assets/Textures"
os.makedirs(output_dir, exist_ok=True)

def save_image(img, name):
    path = os.path.join(output_dir, name)
    img.save(path, format="PNG")
    print(f"Generated texture: {path} ({img.size})")

# 1. Art Terror 01: Silueta en la Niebla (Cuadro vertical 512 x 768)
def make_terror_silueta():
    w, h = 512, 768
    # Background gradient: dark charcoal to deep murky cold cyan-grey
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    for y in range(h):
        factor = y / h
        r = int(10 + 25 * factor)
        g = int(14 + 35 * factor)
        b = int(18 + 40 * factor)
        arr[y, :, 0] = r
        arr[y, :, 1] = g
        arr[y, :, 2] = b
    
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    
    # Layer 1: Distant ghost trees (lighter, blurred)
    for tx in [40, 120, 200, 310, 390, 480]:
        draw.line([(tx, h), (tx + np.random.randint(-15, 15), 180)], fill=(35, 50, 55), width=12)
        # Branches
        draw.line([(tx, 450), (tx - 50, 360)], fill=(32, 48, 52), width=5)
        draw.line([(tx, 400), (tx + 55, 320)], fill=(32, 48, 52), width=4)
        draw.line([(tx, 300), (tx - 40, 240)], fill=(30, 45, 50), width=3)
    
    # Mist band across middle
    mist_mask = Image.new("L", (w, h), 0)
    mdraw = ImageDraw.Draw(mist_mask)
    for _ in range(8):
        my = np.random.randint(280, 520)
        mh = np.random.randint(40, 120)
        mdraw.ellipse([(0, my), (w, my + mh)], fill=70)
    mist_mask = mist_mask.filter(ImageFilter.GaussianBlur(35))
    mist_layer = Image.new("RGB", (w, h), (85, 115, 125))
    img.paste(mist_layer, (0, 0), mist_mask)
    
    # Layer 2: Foreground twisted dark trees
    draw = ImageDraw.Draw(img)
    for tx, tw, top_y in [(80, 22, 100), (430, 26, 80), (330, 16, 140)]:
        draw.line([(tx, h), (tx - 10, top_y)], fill=(12, 15, 18), width=tw)
        draw.line([(tx, 550), (tx + 70, 420)], fill=(12, 15, 18), width=8)
        draw.line([(tx, 450), (tx - 80, 340)], fill=(12, 15, 18), width=7)
        draw.line([(tx - 10, 320), (tx + 50, 220)], fill=(12, 15, 18), width=5)
        draw.line([(tx - 10, 240), (tx - 60, 160)], fill=(12, 15, 18), width=4)
    
    # Central Tall Shadow Entity
    cx = 240
    # Long thin body
    draw.polygon([(cx - 18, h), (cx + 18, h), (cx + 10, 340), (cx - 10, 340)], fill=(5, 6, 8))
    # Slightly tilted eerie elongated head
    draw.ellipse([(cx - 14, 290), (cx + 14, 340)], fill=(5, 6, 8))
    # Unnaturally long dangling arms
    draw.line([(cx - 10, 350), (cx - 38, 480), (cx - 45, 610)], fill=(5, 6, 8), width=7)
    draw.line([(cx + 10, 350), (cx + 36, 475), (cx + 42, 595)], fill=(5, 6, 8), width=7)
    
    # Faint glowing eyes
    draw.ellipse([(cx - 7, 310), (cx - 3, 314)], fill=(240, 220, 160))
    draw.ellipse([(cx + 3, 310), (cx + 7, 314)], fill=(240, 220, 160))
    
    # Dark Vignette around edges
    vig = Image.new("L", (w, h), 0)
    vdraw = ImageDraw.Draw(vig)
    vdraw.rectangle([(0, 0), (w, h)], fill=255)
    vdraw.ellipse([(30, 40), (w - 30, h - 40)], fill=0)
    vig = vig.filter(ImageFilter.GaussianBlur(50))
    
    black = Image.new("RGB", (w, h), (3, 4, 6))
    img.paste(black, (0, 0), vig)
    
    save_image(img, "art_terror_silueta.png")

# 2. Art Terror 02: Ojos en la Penumbra (Cuadro horizontal 768 x 512)
def make_terror_ojos():
    w, h = 768, 512
    img = Image.new("RGB", (w, h), (6, 7, 10))
    draw = ImageDraw.Draw(img)
    
    # Faint silhouettes of abandoned office cubicles / broken blinds in darkness
    for x in range(0, w, 120):
        draw.rectangle([(x + 20, 260), (x + 110, h)], fill=(12, 14, 18))
        draw.line([(x + 20, 260), (x + 110, 260)], fill=(25, 28, 35), width=3)
    
    # Window blinds pattern faintly visible
    for y in range(40, 240, 14):
        draw.line([(80, y), (680, y)], fill=(16, 18, 24), width=3)
    
    # Multiple pairs of glowing creepy eyes lurking
    eye_pairs = [
        # (x, y, size, color, glow_color)
        (380, 210, 12, (255, 30, 30), (180, 0, 0)),     # Center menace
        (160, 290, 8, (255, 180, 20), (180, 100, 0)),   # Under desk left
        (580, 270, 9, (255, 50, 50), (160, 20, 20)),    # Right shadow
        (260, 170, 7, (230, 240, 180), (120, 140, 80)), # Mid upper left
        (490, 150, 10, (255, 210, 40), (190, 140, 0)),  # Upper right
        (80, 380, 6, (255, 40, 40), (160, 0, 0)),       # Low left
        (670, 360, 7, (220, 20, 20), (140, 0, 0)),      # Low right
        (320, 340, 8, (240, 220, 80), (170, 150, 20)),  # Desk right corner
    ]
    
    glow_img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    gdraw = ImageDraw.Draw(glow_img)
    
    for ex, ey, sz, color, glow in eye_pairs:
        sep = sz * 2.2
        # Glow around left eye
        gdraw.ellipse([(ex - sep/2 - sz*2, ey - sz*2), (ex - sep/2 + sz*2, ey + sz*2)], fill=(*glow, 80))
        gdraw.ellipse([(ex + sep/2 - sz*2, ey - sz*2), (ex + sep/2 + sz*2, ey + sz*2)], fill=(*glow, 80))
    
    glow_blurred = glow_img.filter(ImageFilter.GaussianBlur(15))
    img.paste(glow_blurred, (0, 0), glow_blurred)
    
    # Crisp eye cores
    for ex, ey, sz, color, glow in eye_pairs:
        sep = sz * 2.2
        # Left eye
        draw.ellipse([(ex - sep/2 - sz, ey - sz*0.6), (ex - sep/2 + sz, ey + sz*0.6)], fill=color)
        draw.ellipse([(ex - sep/2 - sz*0.3, ey - sz*0.5), (ex - sep/2 + sz*0.3, ey + sz*0.5)], fill=(0, 0, 0)) # Slit pupil
        # Right eye
        draw.ellipse([(ex + sep/2 - sz, ey - sz*0.6), (ex + sep/2 + sz, ey + sz*0.6)], fill=color)
        draw.ellipse([(ex + sep/2 - sz*0.3, ey - sz*0.5), (ex + sep/2 + sz*0.3, ey + sz*0.5)], fill=(0, 0, 0))
        # Small white highlight
        draw.ellipse([(ex - sep/2 - sz*0.4, ey - sz*0.3), (ex - sep/2 - sz*0.1, ey)], fill=(255, 255, 255))
        draw.ellipse([(ex + sep/2 - sz*0.4, ey - sz*0.3), (ex + sep/2 - sz*0.1, ey)], fill=(255, 255, 255))

    # Vignette
    vig = Image.new("L", (w, h), 0)
    vdraw = ImageDraw.Draw(vig)
    vdraw.rectangle([(0, 0), (w, h)], fill=255)
    vdraw.ellipse([(40, 30), (w - 40, h - 30)], fill=0)
    vig = vig.filter(ImageFilter.GaussianBlur(60))
    img.paste(Image.new("RGB", (w, h), (2, 2, 4)), (0, 0), vig)

    save_image(img, "art_terror_ojos.png")

# 3. Art Terror 03: Símbolo Arcano / Ritual (Cuadro cuadrado 512 x 512)
def make_terror_simbolo():
    w, h = 512, 512
    # Aged, distressed bloody parchment texture
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    # Dirty beige/ochre base
    base_r, base_g, base_b = 60, 48, 38
    noise = np.random.normal(0, 12, (h, w))
    arr[:, :, 0] = np.clip(base_r + noise + 10, 0, 255).astype(np.uint8)
    arr[:, :, 1] = np.clip(base_g + noise, 0, 255).astype(np.uint8)
    arr[:, :, 2] = np.clip(base_b + noise - 10, 0, 255).astype(np.uint8)
    
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    
    # Blood / ink splatters
    for _ in range(15):
        bx = np.random.randint(40, w - 40)
        by = np.random.randint(40, h - 40)
        br = np.random.randint(6, 30)
        draw.ellipse([(bx - br, by - br), (bx + br, by + br)], fill=(95, 18, 18))
        # Drips
        draw.line([(bx, by), (bx + np.random.randint(-4, 4), by + np.random.randint(20, 60))], fill=(85, 15, 15), width=2)
    
    # Arcane ritual seal / geometric occult sigil (dark crimson / dried blood)
    cx, cy = 256, 256
    ink_color = (130, 22, 22)
    # Concentric circles
    draw.ellipse([(cx - 180, cy - 180), (cx + 180, cy + 180)], outline=ink_color, width=4)
    draw.ellipse([(cx - 165, cy - 165), (cx + 165, cy + 165)], outline=ink_color, width=2)
    draw.ellipse([(cx - 110, cy - 110), (cx + 110, cy + 110)], outline=ink_color, width=3)
    
    # Seven-pointed or pentagram star lines
    points = []
    num_pts = 7
    for i in range(num_pts):
        angle = -math.pi / 2 + i * 2 * math.pi / num_pts
        r = 160
        points.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    
    for i in range(num_pts):
        p1 = points[i]
        p2 = points[(i + 3) % num_pts]
        draw.line([p1, p2], fill=ink_color, width=3)
        # Small glyph circles at vertices
        draw.ellipse([(p1[0]-7, p1[1]-7), (p1[0]+7, p1[1]+7)], fill=ink_color)
    
    # Inner eye / sigil in center
    draw.arc([(cx - 50, cy - 35), (cx + 50, cy + 35)], 0, 180, fill=ink_color, width=3)
    draw.arc([(cx - 50, cy - 35), (cx + 50, cy + 35)], 180, 360, fill=ink_color, width=3)
    draw.ellipse([(cx - 16, cy - 16), (cx + 16, cy + 16)], fill=ink_color)
    draw.ellipse([(cx - 4, cy - 4), (cx + 4, cy + 4)], fill=(20, 10, 10))
    
    # Redacted text / occult runes around rim
    for i in range(24):
        a = i * (2 * math.pi / 24)
        rx = cx + 138 * math.cos(a)
        ry = cy + 138 * math.sin(a)
        draw.line([(rx - 5, ry), (rx + 5, ry)], fill=ink_color, width=3)
        draw.line([(rx, ry - 4), (rx, ry + 4)], fill=ink_color, width=2)
    
    # Stamped mark: "CASE #899 - DO NOT EXPOSE"
    draw.rectangle([(60, 420), (320, 455)], outline=(110, 20, 20), width=3)
    draw.text((70, 428), "EVIDENCE: FILE 899 - VOID", fill=(110, 20, 20))
    
    # Vignette
    vig = Image.new("L", (w, h), 0)
    vdraw = ImageDraw.Draw(vig)
    vdraw.rectangle([(0, 0), (w, h)], fill=255)
    vdraw.ellipse([(30, 30), (w - 30, h - 30)], fill=0)
    vig = vig.filter(ImageFilter.GaussianBlur(55))
    img.paste(Image.new("RGB", (w, h), (18, 12, 10)), (0, 0), vig)
    
    save_image(img, "art_terror_simbolo.png")

# 4. Art Terror 04: El Pasillo Infinito (Cuadro panorámico 768 x 512)
def make_terror_pasillo():
    w, h = 768, 512
    img = Image.new("RGB", (w, h), (10, 12, 16))
    draw = ImageDraw.Draw(img)
    
    # Vanishing point in the middle-deep
    vx, vy = w // 2, h // 2 - 20
    
    # Floor perspective
    draw.polygon([(0, h), (w, h), (vx + 40, vy + 40), (vx - 40, vy + 40)], fill=(20, 24, 30))
    # Floor checkered tiles in perspective
    for t in [0.9, 0.75, 0.6, 0.45, 0.3, 0.18, 0.08]:
        ly = vy + 40 + int((h - (vy + 40)) * t)
        draw.line([(0, ly), (w, ly)], fill=(32, 38, 48), width=2)
    
    # Ceiling perspective
    draw.polygon([(0, 0), (w, 0), (vx + 40, vy - 40), (vx - 40, vy - 40)], fill=(16, 18, 22))
    
    # Ceiling lights receding (flickering cold neon)
    for t in [0.8, 0.55, 0.35, 0.18]:
        lx = vx
        ly = int(vy - 40 - (vy - 40) * (1 - t) * 0.7)
        lw = int(140 * t)
        draw.rectangle([(lx - lw//2, ly - 6), (lx + lw//2, ly + 6)], fill=(160, 200, 220))
    
    # Walls on left and right
    draw.polygon([(0, 0), (vx - 40, vy - 40), (vx - 40, vy + 40), (0, h)], fill=(26, 30, 36))
    draw.polygon([(w, 0), (vx + 40, vy - 40), (vx + 40, vy + 40), (w, h)], fill=(22, 26, 32))
    
    # Doors on walls receding
    # Left door open showing black void
    draw.polygon([(80, 160), (160, 200), (160, 440), (80, 480)], fill=(3, 3, 5))
    draw.polygon([(70, 150), (80, 160), (80, 480), (70, 490)], fill=(14, 16, 20)) # Door frame
    
    # Right door ajar with faint blood-red light leaking out
    draw.polygon([(w - 180, 210), (w - 100, 175), (w - 100, 455), (w - 180, 420)], fill=(18, 5, 8))
    draw.line([(w - 180, 210), (w - 180, 420)], fill=(220, 30, 30), width=4) # Red light crack
    
    # End of hallway: glowing red emergency exit sign
    draw.rectangle([(vx - 20, vy - 30), (vx + 20, vy - 15)], fill=(220, 20, 20))
    draw.rectangle([(vx - 14, vy - 26), (vx + 14, vy - 19)], fill=(255, 120, 120))
    
    # Faint shadow standing at the end of the hall under the red sign
    draw.polygon([(vx - 6, vy + 38), (vx + 6, vy + 38), (vx + 4, vy - 5), (vx - 4, vy - 5)], fill=(2, 2, 4))
    draw.ellipse([(vx - 5, vy - 15), (vx + 5, vy - 5)], fill=(2, 2, 4))
    
    # Vignette
    vig = Image.new("L", (w, h), 0)
    vdraw = ImageDraw.Draw(vig)
    vdraw.rectangle([(0, 0), (w, h)], fill=255)
    vdraw.ellipse([(50, 40), (w - 50, h - 40)], fill=0)
    vig = vig.filter(ImageFilter.GaussianBlur(55))
    img.paste(Image.new("RGB", (w, h), (4, 5, 7)), (0, 0), vig)
    
    save_image(img, "art_terror_pasillo.png")

# 5. Pizarra de Investigación / Evidence Board (1024 x 640)
def make_evidence_board():
    w, h = 1024, 640
    # Corkboard texture
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    base_r, base_g, base_b = 185, 140, 95
    noise = np.random.normal(0, 18, (h, w))
    arr[:, :, 0] = np.clip(base_r + noise, 0, 255).astype(np.uint8)
    arr[:, :, 1] = np.clip(base_g + noise * 0.9, 0, 255).astype(np.uint8)
    arr[:, :, 2] = np.clip(base_b + noise * 0.8, 0, 255).astype(np.uint8)
    
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    
    # Outer metal frame border
    border_thick = 24
    draw.rectangle([(0, 0), (w, border_thick)], fill=(70, 75, 82))
    draw.rectangle([(0, h - border_thick), (w, h)], fill=(70, 75, 82))
    draw.rectangle([(0, 0), (border_thick, h)], fill=(65, 70, 76))
    draw.rectangle([(w - border_thick, 0), (w, h)], fill=(65, 70, 76))
    
    # Polaroid photos pinned
    photos = [
        # (x, y, label, angle)
        (120, 90, "INCIDENT #1", (15, 20, 25)),
        (380, 80, "VICTIM PROFILE", (220, 200, 180)),
        (720, 95, "SUSPECT 0", (25, 30, 40)),
        (220, 360, "LOCATION - BASEMENT", (30, 35, 45)),
        (560, 340, "SYMBOL FOUND", (80, 20, 20)),
        (780, 350, "SECURITY CAM 04", (10, 15, 20))
    ]
    
    pins = []
    
    for px, py, caption, core_col in photos:
        pw, ph = 140, 170
        # White polaroid paper
        draw.rectangle([(px - 4, py - 4), (px + pw + 4, py + ph + 4)], fill=(245, 242, 235))
        # Inner dark photo area
        draw.rectangle([(px + 10, py + 10), (px + pw - 10, py + ph - 40)], fill=core_col)
        # Silhouette or mark inside photo
        draw.ellipse([(px + 50, py + 35), (px + 90, py + 80)], fill=(0, 0, 0))
        # Red scratch or question mark over photo
        draw.line([(px + 20, py + 20), (px + pw - 20, py + ph - 50)], fill=(220, 20, 20), width=4)
        draw.line([(px + pw - 20, py + 20), (px + 20, py + ph - 50)], fill=(220, 20, 20), width=4)
        # Red push pin at top center
        pin_pt = (px + pw // 2, py + 4)
        pins.append(pin_pt)
    
    # Manila report folders / documents pinned
    docs = [
        (480, 190, 180, 130, (230, 215, 175)),
        (80, 260, 130, 100, (240, 240, 235)),
        (400, 380, 140, 140, (235, 220, 190)),
        (650, 230, 110, 90, (255, 255, 255))
    ]
    for dx, dy, dw, dh, bg in docs:
        draw.rectangle([(dx, dy), (dx + dw, dy + dh)], fill=bg)
        # Black typed lines
        for ly in range(dy + 15, dy + dh - 15, 12):
            draw.line([(dx + 12, ly), (dx + dw - 16, ly)], fill=(120, 120, 120), width=2)
        # Red rubber stamp
        draw.rectangle([(dx + 25, dy + dh - 40), (dx + dw - 25, dy + dh - 15)], outline=(190, 30, 30), width=2)
        pins.append((dx + dw // 2, dy + 6))
    
    # Yellow Post-It notes
    notes = [
        (300, 240, 80, 80, (255, 245, 100)),
        (260, 110, 75, 75, (255, 140, 120)),
        (870, 200, 80, 80, (140, 240, 160)),
        (680, 480, 85, 85, (255, 245, 100))
    ]
    for nx, ny, nw, nh, ncol in notes:
        draw.rectangle([(nx, ny), (nx + nw, ny + nh)], fill=ncol)
        for ly in range(ny + 16, ny + nh - 12, 14):
            draw.line([(nx + 8, ly), (nx + nw - 10, ly)], fill=(60, 60, 60), width=2)
        pins.append((nx + nw // 2, ny + 4))
    
    # Crime investigation RED STRINGS connecting pins
    connections = [
        (0, 1), (1, 2), (0, 3), (1, 4), (2, 5), (4, 5), (6, 1), (7, 3), (8, 4), (10, 1), (11, 2), (12, 5)
    ]
    for i1, i2 in connections:
        if i1 < len(pins) and i2 < len(pins):
            p1, p2 = pins[i1], pins[i2]
            draw.line([p1, p2], fill=(230, 25, 25), width=3)
    
    # Draw pins over strings
    for px, py in pins:
        draw.ellipse([(px - 7, py - 7), (px + 7, py + 7)], fill=(220, 30, 30))
        draw.ellipse([(px - 3, py - 5), (px + 1, py - 2)], fill=(255, 200, 200)) # Highlight
    
    save_image(img, "evidence_board.png")

# 6. Cardboard Box Texture (512 x 512)
def make_cardboard_box():
    w, h = 512, 512
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    base_r, base_g, base_b = 195, 150, 100
    noise = np.random.normal(0, 8, (h, w))
    arr[:, :, 0] = np.clip(base_r + noise, 0, 255).astype(np.uint8)
    arr[:, :, 1] = np.clip(base_g + noise * 0.9, 0, 255).astype(np.uint8)
    arr[:, :, 2] = np.clip(base_b + noise * 0.75, 0, 255).astype(np.uint8)
    
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    
    # Brown packing tape seam down middle
    draw.rectangle([(w // 2 - 25, 0), (w // 2 + 25, h)], fill=(160, 115, 65))
    
    # White label sticker
    draw.rectangle([(60, 120), (220, 260)], fill=(245, 245, 240))
    # Barcode lines
    for bx in range(80, 200, 6):
        bw = np.random.choice([2, 3, 4])
        draw.line([(bx, 140), (bx, 200)], fill=(30, 30, 30), width=bw)
    draw.text((80, 215), "BOX ID: #4092-B", fill=(40, 40, 40))
    
    # Red Stamped Warning: "CONFIDENTIAL / CLASSIFIED"
    draw.rectangle([(250, 180), (470, 250)], outline=(200, 30, 30), width=4)
    draw.text((265, 195), "CLASSIFIED", fill=(200, 30, 30))
    draw.text((275, 220), "DO NOT DISCARD", fill=(200, 30, 30))
    
    # "THIS WAY UP" Arrows
    draw.line([(100, 340), (100, 420)], fill=(40, 40, 40), width=6)
    draw.polygon([(85, 360), (115, 360), (100, 330)], fill=(40, 40, 40))
    draw.line([(140, 340), (140, 420)], fill=(40, 40, 40), width=6)
    draw.polygon([(125, 360), (155, 360), (140, 330)], fill=(40, 40, 40))
    
    # Fragile glass icon
    draw.polygon([(360, 340), (400, 340), (390, 380), (370, 380)], fill=(40, 40, 40))
    draw.rectangle([(378, 380), (382, 420)], fill=(40, 40, 40))
    draw.rectangle([(365, 420), (395, 425)], fill=(40, 40, 40))

    save_image(img, "cardboard_box.png")

# 7. Top Secret Manila Folder Texture (512 x 512)
def make_top_secret():
    w, h = 512, 512
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    base_r, base_g, base_b = 220, 195, 140
    noise = np.random.normal(0, 10, (h, w))
    arr[:, :, 0] = np.clip(base_r + noise, 0, 255).astype(np.uint8)
    arr[:, :, 1] = np.clip(base_g + noise * 0.95, 0, 255).astype(np.uint8)
    arr[:, :, 2] = np.clip(base_b + noise * 0.8, 0, 255).astype(np.uint8)
    
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    
    # Folder tab at top
    draw.rectangle([(20, 10), (180, 50)], fill=(205, 180, 125))
    draw.text((35, 22), "PROJECT: ABERRATION", fill=(60, 50, 40))
    
    # Red diagonal warning stripes on border
    for sx in range(0, w, 40):
        draw.polygon([(sx, 0), (sx + 20, 0), (sx, 20)], fill=(180, 40, 40))
        draw.polygon([(sx, h), (sx + 20, h), (sx + 40, h - 20), (sx + 20, h - 20)], fill=(180, 40, 40))
    
    # Big Bold Red Stamp: "TOP SECRET"
    draw.rectangle([(80, 180), (430, 280)], outline=(210, 25, 25), width=6)
    draw.text((120, 205), "TOP SECRET", fill=(210, 25, 25))
    draw.text((150, 240), "EYES ONLY - LEVEL 5", fill=(210, 25, 25))
    
    # Seal / watermark
    draw.ellipse([(200, 330), (312, 442)], outline=(170, 140, 100), width=4)
    draw.polygon([(256, 345), (295, 420), (217, 420)], outline=(170, 140, 100))
    
    save_image(img, "top_secret_folder.png")

# 8. Fire Extinguisher Label Texture (256 x 512)
def make_extinguisher_label():
    w, h = 256, 512
    img = Image.new("RGB", (w, h), (210, 25, 25))
    draw = ImageDraw.Draw(img)
    
    # White label background
    draw.rectangle([(20, 40), (w - 20, h - 40)], fill=(250, 250, 250))
    
    # Bold header
    draw.rectangle([(20, 40), (w - 20, 110)], fill=(200, 20, 20))
    draw.text((35, 60), "EXTINTOR", fill=(255, 255, 255))
    draw.text((45, 85), "POLVO QUIMICO ABC", fill=(255, 255, 255))
    
    # Pictograms (1: Pull pin, 2: Aim nozzle, 3: Squeeze lever)
    for i, step in enumerate(["1. TIRE PASADOR", "2. APUNTE BOQUILLA", "3. PRESIONE MANIJA"]):
        py = 140 + i * 80
        draw.rectangle([(35, py), (75, py + 40)], fill=(40, 40, 40))
        draw.text((42, py + 12), str(i + 1), fill=(255, 255, 255))
        draw.text((85, py + 14), step, fill=(30, 30, 30))
        draw.line([(30, py + 55), (w - 30, py + 55)], fill=(200, 200, 200), width=1)
    
    # Pressure gauge / inspection seal
    draw.ellipse([(98, 400), (158, 460)], outline=(40, 40, 40), width=3)
    draw.line([(128, 430), (145, 415)], fill=(30, 180, 30), width=3) # Needle in green
    
    save_image(img, "extinguisher_label.png")

# 9. Ceiling Lamp Panel Texture (512 x 256)
def make_ceiling_lamp():
    w, h = 512, 256
    img = Image.new("RGB", (w, h), (45, 48, 52))
    draw = ImageDraw.Draw(img)
    
    # Outer metal frame
    draw.rectangle([(0, 0), (w, h)], fill=(42, 45, 48))
    # Beveled inner inset
    draw.rectangle([(16, 16), (w - 16, h - 16)], fill=(32, 34, 38))
    # White frosted diffuser panel
    draw.rectangle([(26, 26), (w - 26, h - 26)], fill=(235, 240, 245))
    
    # Diffuser prismatic grid / fluorescent tube silhouettes (turned off / low poly clean)
    for x in range(36, w - 36, 16):
        draw.line([(x, 26), (x, h - 26)], fill=(215, 222, 228), width=2)
    # Two fluorescent tubes faintly visible inside
    draw.rectangle([(40, 65), (w - 40, 95)], fill=(245, 250, 255))
    draw.rectangle([(40, 160), (w - 40, 190)], fill=(245, 250, 255))
    
    save_image(img, "lamp_ceiling_panel.png")

if __name__ == "__main__":
    make_terror_silueta()
    make_terror_ojos()
    make_terror_simbolo()
    make_terror_pasillo()
    make_evidence_board()
    make_cardboard_box()
    make_top_secret()
    make_extinguisher_label()
    make_ceiling_lamp()
    print("All textures created successfully!")
