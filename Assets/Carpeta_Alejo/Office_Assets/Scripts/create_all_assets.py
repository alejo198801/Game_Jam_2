import bpy
import bmesh
import math
import os

# Base directory paths
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
TEXTURES_DIR = os.path.join(BASE_DIR, "Assets", "Carpeta_Alejo", "Office_Assets", "Textures")
BLEND_DIR = os.path.join(BASE_DIR, "Assets", "Carpeta_Alejo", "Office_Assets", "Blend")
INDIV_DIR = os.path.join(BLEND_DIR, "Individual")
FBX_DIR = os.path.join(BASE_DIR, "Assets", "Carpeta_Alejo", "Office_Assets", "FBX")
RENDERS_DIR = os.path.join(BASE_DIR, "Assets", "Carpeta_Alejo", "Office_Assets", "Renders")

for d in [INDIV_DIR, FBX_DIR, RENDERS_DIR]:
    os.makedirs(d, exist_ok=True)

# Helper: Clear Scene
def clear_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)

# Helper: Create PBR Material
def create_pbr_material(name, base_color=(0.8, 0.8, 0.8, 1.0), metallic=0.0, roughness=0.5, 
                        transmission=0.0, alpha=1.0, emission_color=(0,0,0,1), emission_strength=0.0,
                        texture_filename=None):
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    
    bsdf = nodes.get("Principled BSDF")
    if not bsdf:
        bsdf = nodes.new(type="ShaderNodeBsdfPrincipled")
        
    bsdf.inputs["Base Color"].default_value = base_color
    if "Metallic" in bsdf.inputs:
        bsdf.inputs["Metallic"].default_value = metallic
    if "Roughness" in bsdf.inputs:
        bsdf.inputs["Roughness"].default_value = roughness
    if "Alpha" in bsdf.inputs:
        bsdf.inputs["Alpha"].default_value = alpha
    if "Transmission Weight" in bsdf.inputs:
        bsdf.inputs["Transmission Weight"].default_value = transmission
    elif "Transmission" in bsdf.inputs:
        bsdf.inputs["Transmission"].default_value = transmission
        
    if emission_strength > 0 and "Emission Color" in bsdf.inputs:
        bsdf.inputs["Emission Color"].default_value = emission_color
        if "Emission Strength" in bsdf.inputs:
            bsdf.inputs["Emission Strength"].default_value = emission_strength

    if texture_filename:
        tex_path = os.path.join(TEXTURES_DIR, texture_filename)
        if os.path.exists(tex_path):
            img = bpy.data.images.load(tex_path)
            tex_node = nodes.new(type="ShaderNodeTexImage")
            tex_node.image = img
            links.new(tex_node.outputs["Color"], bsdf.inputs["Base Color"])
            
    return mat

# Setup Material Library
def get_materials_dict():
    mats = {}
    mats["Wood"] = create_pbr_material("Mat_Wood", base_color=(0.75, 0.5, 0.32, 1), roughness=0.4, texture_filename="wood_desk.png")
    mats["DarkPlastic"] = create_pbr_material("Mat_DarkPlastic", base_color=(0.14, 0.16, 0.18, 1), roughness=0.55, texture_filename="dark_plastic.png")
    mats["BrushedMetal"] = create_pbr_material("Mat_BrushedMetal", base_color=(0.82, 0.84, 0.86, 1), metallic=0.88, roughness=0.25, texture_filename="metal_brushed.png")
    mats["Fabric"] = create_pbr_material("Mat_Fabric", base_color=(0.18, 0.22, 0.28, 1), roughness=0.85, texture_filename="fabric_chair.png")
    mats["Porcelain"] = create_pbr_material("Mat_Porcelain", base_color=(0.95, 0.96, 0.97, 1), roughness=0.12, texture_filename="ceramic_porcelain.png")
    mats["Screen"] = create_pbr_material("Mat_Screen", base_color=(0.1, 0.12, 0.18, 1), roughness=0.2, emission_color=(0.9, 0.95, 1.0, 1), emission_strength=0.35, texture_filename="screen_display.png")
    mats["Microwave"] = create_pbr_material("Mat_Microwave", base_color=(0.85, 0.86, 0.88, 1), roughness=0.3, texture_filename="microwave_front.png")
    mats["WaterJug"] = create_pbr_material("Mat_WaterJug", base_color=(0.2, 0.65, 0.85, 1), roughness=0.15, transmission=0.65, alpha=0.85, texture_filename="water_jug.png")
    mats["Cabinet"] = create_pbr_material("Mat_Cabinet", base_color=(0.3, 0.33, 0.38, 1), roughness=0.45, texture_filename="cabinet_front.png")
    mats["Mirror"] = create_pbr_material("Mat_Mirror", base_color=(0.9, 0.92, 0.95, 1), metallic=0.96, roughness=0.04, texture_filename="mirror.png")
    mats["GlassDark"] = create_pbr_material("Mat_GlassDark", base_color=(0.08, 0.09, 0.1, 1), roughness=0.1, transmission=0.4, alpha=0.9)
    
    # Reference Buttons & Accents
    mats["BtnAmber"] = create_pbr_material("Mat_BtnAmber", base_color=(0.96, 0.69, 0.25, 1), roughness=0.35)
    mats["BtnSalmon"] = create_pbr_material("Mat_BtnSalmon", base_color=(0.91, 0.44, 0.32, 1), roughness=0.35)
    mats["BtnGreen"] = create_pbr_material("Mat_BtnGreen", base_color=(0.32, 0.75, 0.50, 1), roughness=0.35)
    mats["BtnLavender"] = create_pbr_material("Mat_BtnLavender", base_color=(0.49, 0.55, 0.77, 1), roughness=0.35)
    mats["BtnCyan"] = create_pbr_material("Mat_BtnCyan", base_color=(0.41, 0.77, 0.86, 1), roughness=0.3, emission_color=(0.41, 0.77, 0.86, 1), emission_strength=0.2)
    mats["BtnRed"] = create_pbr_material("Mat_BtnRed", base_color=(0.85, 0.2, 0.2, 1), roughness=0.4)
    mats["BtnBlue"] = create_pbr_material("Mat_BtnBlue", base_color=(0.2, 0.5, 0.85, 1), roughness=0.4)
    mats["DarkBody"] = create_pbr_material("Mat_DarkBody", base_color=(0.14, 0.16, 0.18, 1), roughness=0.55)
    mats["Steel"] = create_pbr_material("Mat_Steel", base_color=(0.85, 0.86, 0.88, 1), metallic=0.9, roughness=0.2)
    mats["WhiteEnamel"] = create_pbr_material("Mat_WhiteEnamel", base_color=(0.92, 0.93, 0.94, 1), roughness=0.25)
    
    return mats

# Helper: Create Beveled Box
def create_box(name, size, location, bevel_width=0.015, bevel_segments=1, material=None):
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=location)
    obj = bpy.context.active_object
    obj.name = name
    obj.scale = size
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    
    if bevel_width > 0:
        bev = obj.modifiers.new(name="Bevel", type="BEVEL")
        bev.width = bevel_width
        bev.segments = bevel_segments
        bev.limit_method = "ANGLE"
        bev.angle_limit = math.radians(45)
        bpy.ops.object.modifier_apply(modifier="Bevel")
        
    if material:
        obj.data.materials.append(material)
    return obj

# Helper: Create Cylinder
def create_cylinder(name, radius, depth, vertices=16, location=(0,0,0), rotation=(0,0,0), bevel_width=0.0, material=None):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices, radius=radius, depth=depth, location=location, rotation=rotation
    )
    obj = bpy.context.active_object
    obj.name = name
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    
    if bevel_width > 0:
        bev = obj.modifiers.new(name="Bevel", type="BEVEL")
        bev.width = bevel_width
        bev.segments = 1
        bev.limit_method = "ANGLE"
        bev.angle_limit = math.radians(60)
        bpy.ops.object.modifier_apply(modifier="Bevel")
        
    if material:
        obj.data.materials.append(material)
    return obj

# Helper: UV Smart Project
def apply_smart_uv(obj):
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

# Helper: Join Objects into One Clean Model
def join_objects(obj_list, final_name):
    bpy.ops.object.select_all(action='DESELECT')
    for o in obj_list:
        o.select_set(True)
    bpy.context.view_layer.objects.active = obj_list[0]
    bpy.ops.object.join()
    master_obj = bpy.context.active_object
    master_obj.name = final_name
    apply_smart_uv(master_obj)
    return master_obj

# Helper: Setup Studio Lighting and Camera for Rendering
def setup_studio(focus_obj):
    # Camera
    cam_data = bpy.data.cameras.new("Showcase_Camera")
    cam_data.lens = 50
    cam = bpy.data.objects.new("Showcase_Camera", cam_data)
    bpy.context.collection.objects.link(cam)
    
    bbox = focus_obj.dimensions
    max_dim = max(bbox.x, bbox.y, bbox.z)
    center_z = focus_obj.location.z + bbox.z * 0.5
    
    cam_dist = max(max_dim * 2.2, 1.8)
    cam.location = (cam_dist * 0.85, -cam_dist * 1.1, center_z + cam_dist * 0.55)
    
    # Point at center of object
    dir_vec = (focus_obj.location.x - cam.location.x, focus_obj.location.y - cam.location.y, center_z - cam.location.z)
    rot_z = math.atan2(dir_vec[0], -dir_vec[1])
    hypot = math.hypot(dir_vec[0], dir_vec[1])
    rot_x = math.atan2(hypot, -dir_vec[2]) - math.pi * 0.5
    cam.rotation_euler = (rot_x + math.radians(10), 0, rot_z)
    bpy.context.scene.camera = cam
    
    # 3-Point Lights
    # Key Light
    key_data = bpy.data.lights.new("KeyLight", type='POINT')
    key_data.energy = 250 * cam_dist
    key_data.color = (1.0, 0.98, 0.95)
    key_light = bpy.data.objects.new("KeyLight", key_data)
    key_light.location = (cam_dist * 1.2, -cam_dist * 0.8, center_z + cam_dist * 1.0)
    bpy.context.collection.objects.link(key_light)
    
    # Fill Light
    fill_data = bpy.data.lights.new("FillLight", type='POINT')
    fill_data.energy = 100 * cam_dist
    fill_data.color = (0.85, 0.92, 1.0)
    fill_light = bpy.data.objects.new("FillLight", fill_data)
    fill_light.location = (-cam_dist * 1.0, -cam_dist * 0.7, center_z + cam_dist * 0.4)
    bpy.context.collection.objects.link(fill_light)
    
    # Rim Light
    rim_data = bpy.data.lights.new("RimLight", type='POINT')
    rim_data.energy = 180 * cam_dist
    rim_data.color = (1.0, 1.0, 1.0)
    rim_light = bpy.data.objects.new("RimLight", rim_data)
    rim_light.location = (-cam_dist * 0.6, cam_dist * 1.0, center_z + cam_dist * 0.8)
    bpy.context.collection.objects.link(rim_light)
    
    # Backdrop / Floor
    bpy.ops.mesh.primitive_plane_add(size=max_dim * 10, location=(focus_obj.location.x, focus_obj.location.y, 0))
    floor = bpy.context.active_object
    floor.name = "Studio_Floor"
    floor_mat = create_pbr_material("StudioFloorMat", base_color=(0.12, 0.13, 0.15, 1), roughness=0.6)
    floor.data.materials.append(floor_mat)

# ==========================================
# 11 LOW POLY ASSET GENERATORS
# ==========================================

# 1. Escritorio de Oficina
def build_escritorio(mats):
    objs = []
    # Tabletop with bevel
    top = create_box("Desk_Top", (1.5, 0.8, 0.05), (0, 0, 0.725), bevel_width=0.015, bevel_segments=1, material=mats["Wood"])
    objs.append(top)
    
    # 4 Metal Legs
    for lx in [-0.68, 0.68]:
        for ly in [-0.33, 0.33]:
            leg = create_box(f"Leg_{lx}_{ly}", (0.05, 0.05, 0.70), (lx, ly, 0.35), bevel_width=0.005, material=mats["DarkPlastic"])
            objs.append(leg)
            
    # Metal Crossbars
    cb1 = create_box("Crossbar_L", (0.04, 0.66, 0.04), (-0.68, 0, 0.15), bevel_width=0.005, material=mats["DarkPlastic"])
    cb2 = create_box("Crossbar_Back", (1.36, 0.04, 0.04), (0, 0.33, 0.60), bevel_width=0.005, material=mats["DarkPlastic"])
    objs.extend([cb1, cb2])
    
    # 3-Drawer Pedestal (Right side)
    ped = create_box("Pedestal_Body", (0.38, 0.68, 0.58), (0.47, 0, 0.34), bevel_width=0.01, material=mats["DarkPlastic"])
    objs.append(ped)
    for i in range(3):
        zy = 0.14 + i * 0.18
        drawer = create_box(f"Drawer_{i}", (0.35, 0.02, 0.15), (0.47, -0.345, zy), bevel_width=0.006, material=mats["Wood"])
        handle = create_box(f"Handle_{i}", (0.16, 0.02, 0.015), (0.47, -0.365, zy), bevel_width=0.003, material=mats["BrushedMetal"])
        objs.extend([drawer, handle])
        
    return join_objects(objs, "01_Escritorio_Oficina")

# 2. Silla Ejecutiva de Oficina
def build_silla(mats):
    objs = []
    # 5-Star Wheeled Base
    hub = create_cylinder("Base_Hub", 0.08, 0.06, vertices=10, location=(0, 0, 0.10), bevel_width=0.01, material=mats["DarkPlastic"])
    objs.append(hub)
    for i in range(5):
        ang = i * (2 * math.pi / 5)
        bx = 0.26 * math.cos(ang)
        by = 0.26 * math.sin(ang)
        spoke = create_box(f"Spoke_{i}", (0.28, 0.04, 0.03), (bx * 0.6, by * 0.6, 0.09), bevel_width=0.006, material=mats["DarkPlastic"])
        spoke.rotation_euler = (0, 0, ang)
        bpy.context.view_layer.objects.active = spoke
        bpy.ops.object.transform_apply(rotation=True)
        wheel = create_cylinder(f"Wheel_{i}", 0.032, 0.03, vertices=8, location=(bx, by, 0.035), rotation=(math.pi/2, 0, ang), material=mats["DarkPlastic"])
        objs.extend([spoke, wheel])
        
    # Central Hydraulic Cylinder
    piston = create_cylinder("Piston", 0.032, 0.32, vertices=12, location=(0, 0, 0.28), material=mats["BrushedMetal"])
    objs.append(piston)
    
    # Seat Cushion
    seat = create_box("Seat_Cushion", (0.48, 0.48, 0.08), (0, 0, 0.47), bevel_width=0.02, bevel_segments=2, material=mats["Fabric"])
    objs.append(seat)
    
    # Backrest (Ergonomic curve low poly)
    back_low = create_box("Back_Low", (0.44, 0.06, 0.28), (0, 0.22, 0.64), bevel_width=0.015, material=mats["Fabric"])
    back_up = create_box("Back_Up", (0.42, 0.06, 0.26), (0, 0.235, 0.88), bevel_width=0.015, material=mats["Fabric"])
    spine = create_box("Spine", (0.06, 0.04, 0.45), (0, 0.26, 0.68), bevel_width=0.008, material=mats["DarkPlastic"])
    objs.extend([back_low, back_up, spine])
    
    # Armrests
    for s in [-0.26, 0.26]:
        arm_post = create_box(f"ArmPost_{s}", (0.035, 0.05, 0.20), (s, 0.05, 0.56), bevel_width=0.005, material=mats["DarkPlastic"])
        arm_pad = create_box(f"ArmPad_{s}", (0.07, 0.24, 0.035), (s, 0.05, 0.67), bevel_width=0.01, material=mats["Fabric"])
        objs.extend([arm_post, arm_pad])
        
    return join_objects(objs, "02_Silla_Ejecutiva")

# 3. Computadora y Monitor Setup
def build_computadora(mats):
    objs = []
    # Monitor Stand Base
    base = create_box("Mon_Base", (0.24, 0.18, 0.015), (0, 0, 0.0075), bevel_width=0.004, material=mats["BrushedMetal"])
    pole = create_box("Mon_Pole", (0.04, 0.03, 0.28), (0, 0.04, 0.14), bevel_width=0.004, material=mats["BrushedMetal"])
    objs.extend([base, pole])
    
    # Screen Frame
    frame = create_box("Mon_Frame", (0.58, 0.03, 0.36), (0, 0.01, 0.32), bevel_width=0.01, material=mats["DarkPlastic"])
    # Screen Display Panel
    screen = create_box("Mon_Display", (0.54, 0.005, 0.32), (0, -0.01, 0.32), bevel_width=0.002, material=mats["Screen"])
    objs.extend([frame, screen])
    
    # Keyboard
    kb = create_box("Keyboard", (0.44, 0.14, 0.018), (0, -0.26, 0.009), bevel_width=0.004, material=mats["DarkPlastic"])
    spacebar = create_box("Spacebar", (0.16, 0.02, 0.006), (0, -0.30, 0.02), bevel_width=0.002, material=mats["DarkPlastic"])
    objs.extend([kb, spacebar])
    
    # Mouse
    mouse = create_box("Mouse", (0.065, 0.10, 0.028), (0.28, -0.26, 0.014), bevel_width=0.012, bevel_segments=2, material=mats["DarkPlastic"])
    wheel = create_cylinder("Wheel", 0.008, 0.005, vertices=8, location=(0.28, -0.23, 0.03), rotation=(0, math.pi/2, 0), material=mats["BtnCyan"])
    objs.extend([mouse, wheel])
    
    return join_objects(objs, "03_Computadora_Monitor")

# 4. Archivador Metálico
def build_archivador(mats):
    objs = []
    # Main Outer Body
    body = create_box("Cab_Body", (0.46, 0.58, 1.15), (0, 0, 0.575), bevel_width=0.015, bevel_segments=1, material=mats["DarkPlastic"])
    objs.append(body)
    
    # 3 Large Drawers with Metal Handles and Label Cards
    for i in range(3):
        zy = 0.22 + i * 0.36
        drawer = create_box(f"Cab_Drawer_{i}", (0.41, 0.02, 0.32), (0, -0.295, zy), bevel_width=0.008, material=mats["Cabinet"])
        handle = create_box(f"Cab_Handle_{i}", (0.18, 0.025, 0.02), (0, -0.32, zy + 0.02), bevel_width=0.004, material=mats["BrushedMetal"])
        label_plate = create_box(f"Cab_Label_{i}", (0.10, 0.008, 0.045), (0, -0.31, zy + 0.09), bevel_width=0.002, material=mats["Porcelain"])
        objs.extend([drawer, handle, label_plate])
        
    return join_objects(objs, "04_Archivador_Metalico")

# 5. Dispensador de Agua (Water Cooler)
def build_dispensador_agua(mats):
    objs = []
    # Base Tower Body
    base = create_box("Cooler_Base", (0.36, 0.36, 0.94), (0, 0, 0.47), bevel_width=0.018, bevel_segments=1, material=mats["DarkPlastic"])
    objs.append(base)
    
    # Recessed Dispensing Alcove
    alcove = create_box("Cooler_Alcove", (0.26, 0.14, 0.28), (0, -0.12, 0.64), bevel_width=0.008, material=mats["WhiteEnamel"])
    drip_grill = create_box("Cooler_Grill", (0.24, 0.12, 0.015), (0, -0.12, 0.51), bevel_width=0.003, material=mats["DarkBody"])
    objs.extend([alcove, drip_grill])
    
    # Hot & Cold Push Levers
    hot_tap = create_box("Tap_Hot", (0.025, 0.05, 0.04), (-0.06, -0.10, 0.72), bevel_width=0.003, material=mats["BtnRed"])
    cold_tap = create_box("Tap_Cold", (0.025, 0.05, 0.04), (0.06, -0.10, 0.72), bevel_width=0.003, material=mats["BtnBlue"])
    objs.extend([hot_tap, cold_tap])
    
    # Inverted 20L Water Jug on Top
    jug_collar = create_cylinder("Jug_Collar", 0.14, 0.05, vertices=16, location=(0, 0, 0.965), material=mats["WhiteEnamel"])
    jug_body = create_cylinder("Jug_Body", 0.13, 0.36, vertices=16, location=(0, 0, 1.18), bevel_width=0.02, material=mats["WaterJug"])
    jug_cap = create_cylinder("Jug_Cap", 0.09, 0.05, vertices=16, location=(0, 0, 1.385), bevel_width=0.01, material=mats["WaterJug"])
    objs.extend([jug_collar, jug_body, jug_cap])
    
    return join_objects(objs, "05_Dispensador_Agua")

# 6. Cafetera de Oficina (Exact Homage to User Reference!)
def build_cafetera_oficina(mats):
    objs = []
    # Main Charcoal Beveled Box Body
    body = create_box("Coffee_Body", (0.32, 0.30, 0.42), (0, 0, 0.21), bevel_width=0.022, bevel_segments=2, material=mats["DarkPlastic"])
    objs.append(body)
    
    # Front Recessed Panel
    panel = create_box("Coffee_Panel", (0.25, 0.02, 0.34), (0, -0.145, 0.20), bevel_width=0.008, material=mats["DarkBody"])
    objs.append(panel)
    
    # 4 Colored Buttons (Amber, Salmon, Green, Lavender)
    btn_amber = create_box("Btn_Amber", (0.04, 0.015, 0.04), (-0.075, -0.16, 0.29), bevel_width=0.005, material=mats["BtnAmber"])
    btn_salmon = create_box("Btn_Salmon", (0.04, 0.015, 0.04), (-0.075, -0.16, 0.23), bevel_width=0.005, material=mats["BtnSalmon"])
    btn_green = create_box("Btn_Green", (0.04, 0.015, 0.04), (0.075, -0.16, 0.29), bevel_width=0.005, material=mats["BtnGreen"])
    btn_lavender = create_box("Btn_Lavender", (0.04, 0.015, 0.04), (0.075, -0.16, 0.23), bevel_width=0.005, material=mats["BtnLavender"])
    objs.extend([btn_amber, btn_salmon, btn_green, btn_lavender])
    
    # Large Cyan Rounded Button / Dispenser Pad at Bottom-Center
    btn_cyan = create_box("Btn_Cyan", (0.13, 0.018, 0.11), (0, -0.16, 0.12), bevel_width=0.012, bevel_segments=2, material=mats["BtnCyan"])
    objs.append(btn_cyan)
    
    # Metallic Top Canister (Cylinder) with Stepped Lid
    canister = create_cylinder("Canister_Base", 0.09, 0.22, vertices=16, location=(0, 0.01, 0.53), bevel_width=0.008, material=mats["BrushedMetal"])
    lid_step = create_cylinder("Canister_Lid", 0.075, 0.035, vertices=16, location=(0, 0.01, 0.655), bevel_width=0.005, material=mats["DarkBody"])
    objs.extend([canister, lid_step])
    
    # Angular Hollow Metallic Handle on Right Side (Matching reference photo!)
    # Handle top, bottom, and vertical bar
    h_top = create_box("Handle_Top", (0.05, 0.018, 0.018), (0.115, 0.01, 0.60), bevel_width=0.003, material=mats["BrushedMetal"])
    h_vert = create_box("Handle_Vert", (0.018, 0.018, 0.12), (0.14, 0.01, 0.54), bevel_width=0.003, material=mats["BrushedMetal"])
    h_bot = create_box("Handle_Bot", (0.05, 0.018, 0.018), (0.115, 0.01, 0.48), bevel_width=0.003, material=mats["BrushedMetal"])
    objs.extend([h_top, h_vert, h_bot])
    
    return join_objects(objs, "06_Cafetera_Oficina")

# 7. Microondas de Oficina
def build_microondas(mats):
    objs = []
    # Main Chassis
    body = create_box("Micro_Body", (0.50, 0.35, 0.30), (0, 0, 0.17), bevel_width=0.015, bevel_segments=1, material=mats["BrushedMetal"])
    objs.append(body)
    
    # 4 Rubber Feet
    for fx in [-0.20, 0.20]:
        for fy in [-0.13, 0.13]:
            foot = create_cylinder(f"Foot_{fx}_{fy}", 0.018, 0.02, vertices=8, location=(fx, fy, 0.01), material=mats["DarkPlastic"])
            objs.append(foot)
            
    # Front Door Face (Dark Glass)
    door = create_box("Micro_Door", (0.32, 0.02, 0.25), (-0.065, -0.175, 0.17), bevel_width=0.008, material=mats["GlassDark"])
    # Handle Bar
    handle_post1 = create_box("HPost_1", (0.015, 0.02, 0.015), (0.075, -0.19, 0.25), bevel_width=0.002, material=mats["BrushedMetal"])
    handle_post2 = create_box("HPost_2", (0.015, 0.02, 0.015), (0.075, -0.19, 0.09), bevel_width=0.002, material=mats["BrushedMetal"])
    handle_bar = create_box("HBar", (0.015, 0.015, 0.20), (0.075, -0.205, 0.17), bevel_width=0.003, material=mats["BrushedMetal"])
    objs.extend([door, handle_post1, handle_post2, handle_bar])
    
    # Right Control Panel (Keypad & Digital Display)
    ctrl_panel = create_box("Control_Panel", (0.12, 0.02, 0.25), (0.165, -0.175, 0.17), bevel_width=0.006, material=mats["Microwave"])
    objs.append(ctrl_panel)
    
    return join_objects(objs, "07_Microondas_Oficina")

# 8. Frigobar / Mini-Refrigerador de Oficina
def build_frigobar(mats):
    objs = []
    # Main Body
    body = create_box("Fridge_Body", (0.48, 0.52, 0.82), (0, 0, 0.43), bevel_width=0.02, bevel_segments=2, material=mats["BrushedMetal"])
    objs.append(body)
    
    # Door with beveled profile
    door = create_box("Fridge_Door", (0.46, 0.03, 0.78), (0, -0.265, 0.44), bevel_width=0.015, bevel_segments=1, material=mats["WhiteEnamel"])
    # Modern Bar Handle
    handle = create_box("Fridge_Handle", (0.025, 0.025, 0.24), (0.18, -0.29, 0.58), bevel_width=0.005, material=mats["BrushedMetal"])
    objs.extend([door, handle])
    
    # Bottom Grille & Feet
    grille = create_box("Fridge_Grille", (0.44, 0.02, 0.05), (0, -0.255, 0.035), bevel_width=0.003, material=mats["DarkPlastic"])
    objs.append(grille)
    
    return join_objects(objs, "08_Frigobar_Oficina")

# 9. Inodoro de Baño (Modern Low Poly Toilet)
def build_inodoro(mats):
    objs = []
    # Base Pedestal
    pedestal = create_box("Toilet_Base", (0.24, 0.36, 0.22), (0, 0.06, 0.11), bevel_width=0.02, material=mats["Porcelain"])
    # Ergonomic Bowl
    bowl = create_box("Toilet_Bowl", (0.36, 0.48, 0.20), (0, -0.04, 0.30), bevel_width=0.04, bevel_segments=2, material=mats["Porcelain"])
    # Seat & Lid
    seat = create_box("Toilet_Seat", (0.37, 0.46, 0.025), (0, -0.05, 0.415), bevel_width=0.01, material=mats["WhiteEnamel"])
    lid = create_box("Toilet_Lid", (0.37, 0.46, 0.025), (0, -0.05, 0.44), bevel_width=0.01, material=mats["WhiteEnamel"])
    objs.extend([pedestal, bowl, seat, lid])
    
    # Cistern Water Tank
    tank = create_box("Toilet_Tank", (0.38, 0.20, 0.36), (0, 0.22, 0.58), bevel_width=0.015, bevel_segments=1, material=mats["Porcelain"])
    tank_lid = create_box("Tank_Lid", (0.40, 0.22, 0.03), (0, 0.22, 0.775), bevel_width=0.008, material=mats["Porcelain"])
    # Dual Flush Chrome Button on Top
    flush_btn = create_cylinder("Flush_Btn", 0.03, 0.012, vertices=12, location=(0, 0.22, 0.795), material=mats["BrushedMetal"])
    objs.extend([tank, tank_lid, flush_btn])
    
    return join_objects(objs, "09_Inodoro_Bano")

# 10. Lavamanos con Mueble y Espejo (Bathroom Vanity & Mirror)
def build_lavamanos(mats):
    objs = []
    # Vanity Cabinet Unit
    cab = create_box("Vanity_Base", (0.64, 0.48, 0.72), (0, 0, 0.36), bevel_width=0.015, material=mats["Wood"])
    objs.append(cab)
    
    # 2 Cabinet Doors with Metal Handles
    for dx in [-0.15, 0.15]:
        door = create_box(f"Vanity_Door_{dx}", (0.28, 0.02, 0.62), (dx, -0.245, 0.37), bevel_width=0.008, material=mats["DarkPlastic"])
        hnd = create_box(f"Vanity_Hnd_{dx}", (0.015, 0.02, 0.12), (dx * 0.4, -0.265, 0.50), bevel_width=0.003, material=mats["BrushedMetal"])
        objs.extend([door, hnd])
        
    # Ceramic Sink Basin on Top
    basin = create_box("Sink_Basin", (0.66, 0.50, 0.08), (0, 0, 0.76), bevel_width=0.02, bevel_segments=1, material=mats["Porcelain"])
    # Recessed bowl cavity
    cavity = create_box("Sink_Cavity", (0.44, 0.32, 0.04), (0, -0.03, 0.77), bevel_width=0.015, material=mats["WhiteEnamel"])
    objs.extend([basin, cavity])
    
    # Chrome Gooseneck Faucet & Mixer Lever
    faucet_base = create_cylinder("Faucet_Base", 0.02, 0.08, vertices=10, location=(0, 0.15, 0.84), material=mats["BrushedMetal"])
    faucet_neck = create_box("Faucet_Neck", (0.025, 0.08, 0.025), (0, 0.11, 0.90), bevel_width=0.005, material=mats["BrushedMetal"])
    faucet_spout = create_cylinder("Faucet_Spout", 0.012, 0.03, vertices=8, location=(0, 0.07, 0.88), rotation=(0, 0, 0), material=mats["BrushedMetal"])
    mixer_lever = create_box("Mixer_Lever", (0.01, 0.04, 0.01), (0.03, 0.15, 0.86), bevel_width=0.002, material=mats["BrushedMetal"])
    objs.extend([faucet_base, faucet_neck, faucet_spout, mixer_lever])
    
    # Framed Wall Mirror (Positioned above sink)
    mirror_frame = create_box("Mirror_Frame", (0.54, 0.03, 0.74), (0, 0.18, 1.40), bevel_width=0.01, material=mats["DarkPlastic"])
    mirror_glass = create_box("Mirror_Glass", (0.48, 0.005, 0.68), (0, 0.165, 1.40), bevel_width=0.002, material=mats["Mirror"])
    objs.extend([mirror_frame, mirror_glass])
    
    return join_objects(objs, "10_Lavamanos_Mueble")

# 11. Accesorios de Baño (Dispensador de Papel, Jabón y Papelera)
def build_accesorios_bano(mats):
    objs = []
    # 1. Jumbo Wall Toilet Paper Dispenser
    tp_body = create_cylinder("TP_Body", 0.13, 0.12, vertices=16, location=(-0.35, 0, 0.50), rotation=(math.pi/2, 0, 0), bevel_width=0.01, material=mats["DarkPlastic"])
    tp_slot = create_box("TP_Slot", (0.08, 0.01, 0.02), (-0.35, -0.065, 0.40), bevel_width=0.002, material=mats["WhiteEnamel"])
    objs.extend([tp_body, tp_slot])
    
    # 2. Wall Liquid Soap Dispenser
    soap_body = create_box("Soap_Body", (0.10, 0.09, 0.22), (0.35, 0, 0.50), bevel_width=0.01, material=mats["DarkPlastic"])
    soap_win = create_box("Soap_Win", (0.03, 0.01, 0.12), (0.35, -0.048, 0.50), bevel_width=0.003, material=mats["BtnCyan"])
    soap_btn = create_box("Soap_Btn", (0.06, 0.02, 0.03), (0.35, -0.05, 0.42), bevel_width=0.003, material=mats["BrushedMetal"])
    objs.extend([soap_body, soap_win, soap_btn])
    
    # 3. Step Waste Bin / Trash Can
    bin_body = create_cylinder("Bin_Body", 0.13, 0.34, vertices=16, location=(0, 0, 0.17), bevel_width=0.01, material=mats["BrushedMetal"])
    bin_lid = create_cylinder("Bin_Lid", 0.135, 0.03, vertices=16, location=(0, 0, 0.355), bevel_width=0.005, material=mats["DarkPlastic"])
    bin_pedal = create_box("Bin_Pedal", (0.05, 0.05, 0.015), (0, -0.15, 0.02), bevel_width=0.003, material=mats["DarkPlastic"])
    objs.extend([bin_body, bin_lid, bin_pedal])
    
    return join_objects(objs, "11_Dispensadores_Bano")

# ==========================================
# MASTER GENERATOR & EXPORTER
# ==========================================
def main():
    print("==========================================")
    print("STARTING LOW POLY OFFICE ASSETS CREATION")
    print("==========================================")
    
    builders = [
        ("01_Escritorio_Oficina", build_escritorio),
        ("02_Silla_Ejecutiva", build_silla),
        ("03_Computadora_Monitor", build_computadora),
        ("04_Archivador_Metalico", build_archivador),
        ("05_Dispensador_Agua", build_dispensador_agua),
        ("06_Cafetera_Oficina", build_cafetera_oficina),
        ("07_Microondas_Oficina", build_microondas),
        ("08_Frigobar_Oficina", build_frigobar),
        ("09_Inodoro_Bano", build_inodoro),
        ("10_Lavamanos_Mueble", build_lavamanos),
        ("11_Dispensadores_Bano", build_accesorios_bano),
    ]
    
    # 1. Generate Individual Assets, save .blend, export .fbx, render preview image
    for name, builder_func in builders:
        clear_scene()
        mats = get_materials_dict()
        obj = builder_func(mats)
        
        # Save individual .blend file
        blend_path = os.path.join(INDIV_DIR, f"{name}.blend")
        bpy.ops.wm.save_as_mainfile(filepath=blend_path)
        print(f"[OK] Saved Blender File: {blend_path}")
        
        # Export FBX for Unity
        fbx_path = os.path.join(FBX_DIR, f"{name}.fbx")
        bpy.ops.export_scene.fbx(filepath=fbx_path, use_selection=False, apply_scale_options='FBX_SCALE_ALL')
        print(f"[OK] Exported FBX File: {fbx_path}")
        
        # Render Beauty Shot
        setup_studio(obj)
        render_path = os.path.join(RENDERS_DIR, f"{name}.png")
        bpy.context.scene.render.resolution_x = 800
        bpy.context.scene.render.resolution_y = 600
        bpy.context.scene.render.filepath = render_path
        bpy.ops.render.render(write_still=True)
        print(f"[OK] Rendered Image: {render_path}")
        
    # 2. Build Master Showroom Scene (.blend) with all 11 models arranged in a gallery
    clear_scene()
    mats = get_materials_dict()
    
    # Grid Arrangement
    # Row 1: Office (Desk, Chair, Computer, Filing Cabinet)
    # Row 2: Kitchen (Water Cooler, Coffee Machine, Microwave, Mini Fridge)
    # Row 3: Bathroom (Toilet, Sink & Mirror, Bathroom Accessories)
    
    showroom_items = [
        # (func, location_x, location_y)
        (build_escritorio, -2.4, 1.2),
        (build_silla, -0.6, 1.2),
        (build_computadora, 1.0, 1.2),
        (build_archivador, 2.4, 1.2),
        
        (build_dispensador_agua, -2.4, -0.4),
        (build_cafetera_oficina, -0.8, -0.4),
        (build_microondas, 0.7, -0.4),
        (build_frigobar, 2.2, -0.4),
        
        (build_inodoro, -1.8, -2.0),
        (build_lavamanos, 0.2, -2.0),
        (build_accesorios_bano, 2.2, -2.0),
    ]
    
    all_objs = []
    for func, px, py in showroom_items:
        model = func(mats)
        model.location = (px, py, 0)
        bpy.ops.object.transform_apply(location=True, rotation=False, scale=False)
        all_objs.append(model)
        
    # Pedestal / Floor for showroom
    bpy.ops.mesh.primitive_plane_add(size=12.0, location=(0, -0.3, 0))
    floor = bpy.context.active_object
    floor.name = "Showroom_Floor"
    floor_mat = mats["DarkBody"]
    floor.data.materials.append(floor_mat)
    
    # Camera for full showroom view
    cam_data = bpy.data.cameras.new("Master_Showroom_Camera")
    cam_data.lens = 32
    cam = bpy.data.objects.new("Master_Showroom_Camera", cam_data)
    bpy.context.collection.objects.link(cam)
    cam.location = (0.2, -5.8, 3.4)
    cam.rotation_euler = (math.radians(64), 0, math.radians(2))
    bpy.context.scene.camera = cam
    
    # Showroom Lighting
    # 1. Main Overhead Sun/Area
    sun_data = bpy.data.lights.new("Showroom_Sun", type='SUN')
    sun_data.energy = 3.5
    sun_data.color = (1.0, 0.98, 0.95)
    sun = bpy.data.objects.new("Showroom_Sun", sun_data)
    sun.rotation_euler = (math.radians(50), math.radians(20), math.radians(-30))
    bpy.context.collection.objects.link(sun)
    
    # 2. Key Spotlights for zones
    for lx in [-2.0, 0.0, 2.0]:
        spot_data = bpy.data.lights.new(f"Spot_{lx}", type='POINT')
        spot_data.energy = 400
        spot_data.color = (0.9, 0.95, 1.0)
        spot = bpy.data.objects.new(f"Spot_{lx}", spot_data)
        spot.location = (lx, -0.5, 3.0)
        bpy.context.collection.objects.link(spot)
        
    # Save Master Showroom .blend
    master_blend_path = os.path.join(BLEND_DIR, "Office_Assets_Showroom.blend")
    bpy.ops.wm.save_as_mainfile(filepath=master_blend_path)
    print(f"\n[OK] MASTER SHOWROOM SAVED: {master_blend_path}")
    
    # Also export Master Combined FBX
    master_fbx_path = os.path.join(FBX_DIR, "Office_Assets_All_Combined.fbx")
    bpy.ops.export_scene.fbx(filepath=master_fbx_path, use_selection=False, apply_scale_options='FBX_SCALE_ALL')
    print(f"[OK] MASTER COMBINED FBX SAVED: {master_fbx_path}")
    
    # Render Master Overview Image
    showroom_render = os.path.join(RENDERS_DIR, "Showroom_Overview.png")
    bpy.context.scene.render.resolution_x = 1280
    bpy.context.scene.render.resolution_y = 720
    bpy.context.scene.render.filepath = showroom_render
    bpy.ops.render.render(write_still=True)
    print(f"[OK] MASTER SHOWROOM RENDERED: {showroom_render}\n")
    
    print("==========================================")
    print("ALL 11 ASSETS SUCCESSFULLY GENERATED & SAVED!")
    print("==========================================")

if __name__ == "__main__":
    main()
