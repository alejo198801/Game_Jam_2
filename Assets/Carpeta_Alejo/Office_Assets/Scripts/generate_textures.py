import os
import numpy as np
from PIL import Image, ImageDraw, ImageFont

output_dir = "Assets/Carpeta_Alejo/Office_Assets/Textures"
os.makedirs(output_dir, exist_ok=True)

def save_image(img, name):
    path = os.path.join(output_dir, name)
    img.save(path, format="PNG")
    print(f"Saved: {path}")

# 1. Wood Desk Texture (512x512)
def make_wood():
    w, h = 512, 512
    # Base warm oak tone
    base_r, base_g, base_b = 190, 135, 85
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    
    y = np.linspace(0, 20 * np.pi, h)
    x = np.linspace(0, 4 * np.pi, w)
    Y, X = np.meshgrid(y, x, indexing='ij')
    
    # Grain pattern with sine waves and noise
    grain = np.sin(Y + 0.3 * np.sin(X) + 0.5 * np.sin(Y * 0.2))
    noise = np.random.normal(0, 0.08, (h, w))
    grain_val = np.clip(0.5 + 0.35 * grain + noise, 0, 1)
    
    arr[:, :, 0] = np.clip(base_r * (0.8 + 0.3 * grain_val), 0, 255).astype(np.uint8)
    arr[:, :, 1] = np.clip(base_g * (0.8 + 0.3 * grain_val), 0, 255).astype(np.uint8)
    arr[:, :, 2] = np.clip(base_b * (0.8 + 0.3 * grain_val), 0, 255).astype(np.uint8)
    
    img = Image.fromarray(arr)
    # Add subtle planks / seams
    draw = ImageDraw.Draw(img)
    for px in [128, 256, 384]:
        draw.line([(px, 0), (px, h)], fill=(120, 80, 45), width=2)
    save_image(img, "wood_desk.png")

# 2. Brushed Metal Texture (512x512)
def make_brushed_metal():
    w, h = 512, 512
    base = 205
    # Horizontal streaks
    streaks = np.random.normal(0, 12, (h, 1))
    streaks = np.repeat(streaks, w, axis=1)
    noise = np.random.normal(0, 6, (h, w))
    val = np.clip(base + streaks + noise, 140, 245).astype(np.uint8)
    
    arr = np.stack([val, val, np.clip(val + 3, 0, 255).astype(np.uint8)], axis=-1)
    img = Image.fromarray(arr)
    save_image(img, "metal_brushed.png")

# 3. Dark Plastic Texture (512x512)
def make_dark_plastic():
    w, h = 512, 512
    base = 45
    noise = np.random.normal(0, 5, (h, w))
    val = np.clip(base + noise, 30, 65).astype(np.uint8)
    arr = np.stack([val, np.clip(val + 2, 0, 255).astype(np.uint8), np.clip(val + 4, 0, 255).astype(np.uint8)], axis=-1)
    img = Image.fromarray(arr)
    save_image(img, "dark_plastic.png")

# 4. Fabric Cushion Texture (512x512)
def make_fabric():
    w, h = 512, 512
    y, x = np.indices((h, w))
    weave = ((x // 3 % 2) ^ (y // 3 % 2)) * 18
    noise = np.random.normal(0, 4, (h, w))
    val = np.clip(55 + weave + noise, 40, 95).astype(np.uint8)
    arr = np.stack([val, np.clip(val + 4, 0, 255).astype(np.uint8), np.clip(val + 10, 0, 255).astype(np.uint8)], axis=-1)
    img = Image.fromarray(arr)
    save_image(img, "fabric_chair.png")

# 5. Screen UI Texture (512x512)
def make_screen_ui():
    img = Image.new("RGB", (512, 512), color=(26, 30, 42))
    draw = ImageDraw.Draw(img)
    # Header window bar
    draw.rectangle([(0, 0), (512, 36)], fill=(38, 44, 60))
    # Window controls (mac/linux style dots)
    draw.ellipse([(14, 11), (26, 23)], fill=(235, 87, 87))
    draw.ellipse([(34, 11), (46, 23)], fill=(242, 201, 76))
    draw.ellipse([(54, 11), (66, 23)], fill=(39, 174, 96))
    draw.rectangle([(100, 10), (320, 26)], fill=(50, 58, 78))
    
    # Left sidebar
    draw.rectangle([(0, 36), (80, 512)], fill=(32, 37, 52))
    for iy in range(55, 380, 45):
        draw.rectangle([(18, iy), (62, iy + 24)], fill=(65, 75, 100))
        
    # Main dashboard area
    # KPI Cards
    draw.rectangle([(100, 55), (280, 140)], fill=(35, 42, 60), outline=(60, 70, 95), width=2)
    draw.rectangle([(115, 70), (210, 85)], fill=(86, 204, 242))
    draw.rectangle([(115, 95), (265, 125)], fill=(45, 55, 78))
    
    draw.rectangle([(300, 55), (490, 140)], fill=(35, 42, 60), outline=(60, 70, 95), width=2)
    draw.rectangle([(315, 70), (410, 85)], fill=(111, 207, 151))
    draw.rectangle([(315, 95), (465, 125)], fill=(45, 55, 78))
    
    # Graph area with bar chart
    draw.rectangle([(100, 160), (490, 330)], fill=(32, 38, 54), outline=(60, 70, 95), width=2)
    bars = [40, 75, 110, 85, 130, 100, 145, 120]
    bar_x = 125
    colors = [(86, 204, 242), (111, 207, 151), (242, 153, 74), (187, 107, 217)]
    for i, b in enumerate(bars):
        c = colors[i % len(colors)]
        draw.rectangle([(bar_x, 310 - b), (bar_x + 30, 310)], fill=c)
        bar_x += 45
        
    # Code / log lines at bottom
    draw.rectangle([(100, 350), (490, 490)], fill=(20, 24, 34), outline=(50, 60, 80), width=2)
    line_colors = [(130, 170, 255), (199, 146, 234), (195, 232, 141), (247, 140, 108), (137, 221, 255)]
    ly = 368
    for i in range(5):
        w_len = [180, 260, 140, 310, 220][i]
        draw.rectangle([(120, ly), (120 + w_len, ly + 14)], fill=line_colors[i % len(line_colors)])
        ly += 24
        
    save_image(img, "screen_display.png")

# 6. Ceramic Porcelain Texture (512x512)
def make_ceramic():
    w, h = 512, 512
    base = 244
    noise = np.random.normal(0, 3, (h, w))
    val = np.clip(base + noise, 230, 255).astype(np.uint8)
    arr = np.stack([val, np.clip(val + 1, 0, 255).astype(np.uint8), np.clip(val + 2, 0, 255).astype(np.uint8)], axis=-1)
    img = Image.fromarray(arr)
    save_image(img, "ceramic_porcelain.png")

# 7. Microwave Front Panel Texture (512x512)
def make_microwave():
    img = Image.new("RGB", (512, 512), color=(220, 222, 225))
    draw = ImageDraw.Draw(img)
    # Glass door on left
    draw.rectangle([(20, 30), (340, 480)], fill=(28, 30, 35), outline=(180, 185, 190), width=8)
    # Glass inner view
    draw.rectangle([(40, 50), (320, 460)], fill=(20, 22, 26))
    # Dot matrix on glass
    for y in range(70, 440, 18):
        for x in range(60, 300, 18):
            draw.ellipse([(x, y), (x+3, y+3)], fill=(45, 50, 55))
    # Door handle groove
    draw.rectangle([(325, 120), (335, 390)], fill=(150, 155, 160))
    
    # Right control panel
    draw.rectangle([(360, 30), (495, 480)], fill=(35, 38, 44), outline=(180, 185, 190), width=4)
    # Digital LED display (Green 12:00)
    draw.rectangle([(375, 50), (480, 110)], fill=(10, 15, 10))
    # Draw segments of 12:00
    # 1
    draw.rectangle([(390, 60), (394, 98)], fill=(50, 230, 90))
    # 2
    draw.rectangle([(404, 60), (422, 64)], fill=(50, 230, 90))
    draw.rectangle([(418, 64), (422, 78)], fill=(50, 230, 90))
    draw.rectangle([(404, 78), (422, 82)], fill=(50, 230, 90))
    draw.rectangle([(404, 82), (408, 96)], fill=(50, 230, 90))
    draw.rectangle([(404, 96), (422, 100)], fill=(50, 230, 90))
    # Colon
    draw.rectangle([(429, 70), (432, 74)], fill=(50, 230, 90))
    draw.rectangle([(429, 86), (432, 90)], fill=(50, 230, 90))
    # 0
    draw.rectangle([(438, 60), (454, 100)], fill=(10, 15, 10), outline=(50, 230, 90), width=3)
    # 0
    draw.rectangle([(460, 60), (476, 100)], fill=(10, 15, 10), outline=(50, 230, 90), width=3)
    
    # Keypad buttons
    for r in range(4):
        for c in range(3):
            bx = 378 + c * 36
            by = 135 + r * 42
            draw.rectangle([(bx, by), (bx + 28, by + 30)], fill=(55, 60, 70), outline=(80, 85, 95), width=2)
            
    # Start and Stop big buttons
    draw.rectangle([(378, 330), (430, 375)], fill=(39, 174, 96), outline=(20, 120, 60), width=2) # Start
    draw.rectangle([(440, 330), (488, 375)], fill=(235, 87, 87), outline=(160, 40, 40), width=2)  # Stop
    
    # Rotary dial knob at bottom
    draw.ellipse([(405, 400), (460, 455)], fill=(70, 75, 85), outline=(130, 135, 145), width=3)
    draw.line([(432, 405), (432, 420)], fill=(220, 220, 220), width=3)
    
    save_image(img, "microwave_front.png")

# 8. Water Jug Texture (512x512)
def make_water_jug():
    w, h = 512, 512
    arr = np.zeros((h, w, 4), dtype=np.uint8)
    
    # Cyan-blue water gradient
    for y in range(h):
        ratio = y / h
        r = int(50 + 20 * ratio)
        g = int(160 + 40 * ratio)
        b = int(220 + 30 * ratio)
        a = 210 # semi-translucent
        arr[y, :, 0] = r
        arr[y, :, 1] = g
        arr[y, :, 2] = b
        arr[y, :, 3] = a
        
    img = Image.fromarray(arr, mode="RGBA")
    draw = ImageDraw.Draw(img)
    
    # Ribbed indentations
    for ry in [110, 210, 310, 410]:
        draw.line([(0, ry), (w, ry)], fill=(30, 110, 170, 240), width=8)
        draw.line([(0, ry + 4), (w, ry + 4)], fill=(120, 220, 255, 200), width=4)
        
    # Water logo in center
    draw.ellipse([(206, 170), (306, 270)], fill=(255, 255, 255, 160))
    # Drop shape
    draw.polygon([(256, 190), (225, 245), (287, 245)], fill=(40, 140, 220, 220))
    draw.ellipse([(225, 220), (287, 260)], fill=(40, 140, 220, 220))
    
    save_image(img, "water_jug.png")

# 9. Cabinet Metal Drawer Front (512x512)
def make_cabinet_front():
    img = Image.new("RGB", (512, 512), color=(75, 82, 92))
    draw = ImageDraw.Draw(img)
    
    # 3 Drawer faces
    drawer_h = 160
    labels = ["PROYECTOS", "FINANZAS", "ARCHIVOS"]
    for i in range(3):
        dy = 12 + i * 164
        # Drawer panel bevel
        draw.rectangle([(16, dy), (496, dy + 152)], fill=(85, 93, 104), outline=(50, 55, 62), width=3)
        # Inner recessed edge
        draw.rectangle([(24, dy + 8), (488, dy + 144)], outline=(110, 120, 132), width=2)
        
        # Label card slot (metal plate with label card)
        draw.rectangle([(206, dy + 25), (306, dy + 65)], fill=(225, 228, 230), outline=(40, 45, 50), width=2)
        # Label lines
        draw.line([(220, dy + 45), (292, dy + 45)], fill=(60, 65, 75), width=3)
        
        # Horizontal metal bar handle
        draw.rectangle([(170, dy + 85), (342, dy + 105)], fill=(195, 200, 205), outline=(130, 135, 140), width=2)
        draw.rectangle([(180, dy + 92), (332, dy + 98)], fill=(150, 155, 160))
        
        # Keyhole on the right
        draw.ellipse([(440, dy + 35), (452, dy + 47)], fill=(40, 45, 50))
        draw.polygon([(444, dy + 43), (448, dy + 43), (449, dy + 55), (443, dy + 55)], fill=(40, 45, 50))
        
    save_image(img, "cabinet_front.png")

# 10. Coffee Machine Front Panel (Style from user's image)
def make_coffee_machine_panel():
    img = Image.new("RGB", (512, 512), color=(36, 40, 44))
    draw = ImageDraw.Draw(img)
    
    # Front recessed control panel
    draw.rectangle([(30, 30), (482, 482)], fill=(32, 35, 38), outline=(22, 24, 26), width=6)
    
    # 4 Colored accent buttons matching image exactly:
    # Top-Left: Amber Yellow
    draw.rounded_rectangle([(65, 80), (165, 160)], radius=12, fill=(244, 176, 60), outline=(200, 135, 30), width=4)
    # Bottom-Left: Coral / Salmon Red
    draw.rounded_rectangle([(65, 190), (165, 270)], radius=12, fill=(231, 111, 81), outline=(190, 75, 50), width=4)
    
    # Top-Right: Mint Pastel Green
    draw.rounded_rectangle([(347, 80), (447, 160)], radius=12, fill=(82, 190, 128), outline=(50, 150, 90), width=4)
    # Bottom-Right: Periwinkle Lavender Blue
    draw.rounded_rectangle([(347, 190), (447, 270)], radius=12, fill=(125, 140, 196), outline=(90, 105, 160), width=4)
    
    # Center-Bottom: Large rounded Cyan Dispenser Pad / Display
    draw.rounded_rectangle([(130, 310), (382, 460)], radius=24, fill=(104, 197, 219), outline=(70, 160, 180), width=6)
    
    save_image(img, "coffee_machine_front.png")

# 11. Mirror Reflection Texture
def make_mirror():
    w, h = 512, 512
    arr = np.zeros((h, w, 3), dtype=np.uint8)
    for y in range(h):
        for x in range(w):
            diag = (x + y) / (w + h)
            val = int(200 + 45 * np.sin(diag * np.pi * 1.5))
            arr[y, x, 0] = np.clip(val - 10, 0, 255)
            arr[y, x, 1] = np.clip(val + 5, 0, 255)
            arr[y, x, 2] = np.clip(val + 20, 0, 255)
    img = Image.fromarray(arr)
    draw = ImageDraw.Draw(img)
    # Rim frame
    draw.rectangle([(0, 0), (w-1, h-1)], outline=(60, 65, 75), width=12)
    # Diagonal light streak
    for off in range(-40, 40, 6):
        draw.line([(100 + off, 40), (400 + off, 470)], fill=(255, 255, 255, 60), width=4)
    save_image(img, "mirror.png")

# 12. Tile Texture for Bathroom / Kitchen
def make_tiles():
    img = Image.new("RGB", (512, 512), color=(210, 215, 220))
    draw = ImageDraw.Draw(img)
    tile_size = 128
    for y in range(0, 512, tile_size):
        for x in range(0, 512, tile_size):
            draw.rectangle([(x + 4, y + 4), (x + tile_size - 4, y + tile_size - 4)], fill=(245, 248, 250))
    save_image(img, "subway_tiles.png")

if __name__ == "__main__":
    make_wood()
    make_brushed_metal()
    make_dark_plastic()
    make_fabric()
    make_screen_ui()
    make_ceramic()
    make_microwave()
    make_water_jug()
    make_cabinet_front()
    make_coffee_machine_panel()
    make_mirror()
    make_tiles()
    print("All 12 textures generated successfully!")
