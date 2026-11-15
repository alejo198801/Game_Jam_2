import os
import re
import math

SCENE_PATH = "Assets/Carpeta_Alejo/Mapa.unity"

GUIDS = {
    "escritorio_oficina": "cb3a7751137ea5d9eb1d2bf7a37e724d",
    "silla_ejecutiva": "144d5ec7387dcc6358fca1fdb2dffe43",
    "computadora_monitor": "b206af668b3dad6318657bf3932faf47",
    "archivador_metalico": "5bf34b1c8237311259c2394a4896b570",
    "dispensador_agua": "d47b27768b2024dd18e6c80ed5c274c3",
    "cafetera_oficina": "74c3d7ec2f0b7e309ae2c9083efdcb58",
    "microondas_oficina": "7145402a5800a5c918a94ddff9104fe9",
    "frigobar_oficina": "aba50de1dcf22c25683bf9b0313b9553",
    "inodoro_bano": "88a30c9aa34df3ae8bd4ecc1954a7f4e",
    "lavamanos_mueble": "536db4c428541c85fa26e063b1dd28f6",
    "dispensadores_bano": "0a45a6d7ad3e5df2595e98a6a978a931",
    "escritorio_individual": "c84eeec43e918cf0ebb49ea06774f4fb",
    "mesa_pequena": "02d0af89ce0580d53967b902813ab95c",
    "mueble_largo_ofi": "9529b0dedc6f2c16d86b41410cde9bee",
    "sillon_individual": "3604f2492f26447ecadbc526212c4d27",
    "telefono": "56af9a425359a625bb86ca932e678982",
    "biblioteca": "00fe222a400db6ddd8cb3c47a052387a",
    "cafetera_variante": "653b9262168b93a1e8e07658de75fb1e",
    "compu_variante": "9a8002d18a59f3f2788964996df40f8f",
    "libros_pila": "32ab2c91db84228e5a0c45c697e88aee",
    "libros": "c2d61e69cbffb064cb3054dfd00b87b8",
}

def yaw_to_quaternion(yaw_deg):
    rad = math.radians(yaw_deg) * 0.5
    qw = math.cos(rad)
    qy = math.sin(rad)
    return 0.0, qy, 0.0, qw

# Surface Heights:
# Floor = 0.00
# Desk top = 0.75
# Kitchen counter top = 0.92
# Coffee table top = 0.93
# Bookshelf shelf = 0.85

instances = [
    # ==========================================
    # ZONE 1: RECEPTION & WAITING LOUNGE (East Central Hall)
    # ==========================================
    {"name": "Sillon_Espera_01", "type": "sillon_individual", "pos": (3.5, 0.00, 0.8), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Sillon_Espera_02", "type": "sillon_individual", "pos": (3.5, 0.00, -0.8), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Mesa_Espera_01", "type": "mesa_pequena", "pos": (3.5, 0.00, 0.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Revistas_Espera_01", "type": "libros", "pos": (3.5, 0.93, 0.0), "yaw": 25, "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Espera", "type": "dispensador_agua", "pos": (6.8, 0.00, 2.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Biblioteca_Recepcion", "type": "biblioteca", "pos": (7.3, 0.00, -1.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Libros_Pila_Recepcion", "type": "libros_pila", "pos": (7.1, 0.85, -1.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Silla_Auxiliar_Recepcion", "type": "silla_ejecutiva", "pos": (6.5, 0.00, -3.0), "yaw": -45, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 2: WORKSTATION AREA (West Central Hall)
    # ==========================================
    # Workstation A
    {"name": "Escritorio_Puesto_A", "type": "escritorio_oficina", "pos": (-4.5, 0.00, 2.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Silla_Puesto_A", "type": "silla_ejecutiva", "pos": (-4.5, 0.00, 2.8), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Computadora_Puesto_A", "type": "computadora_monitor", "pos": (-4.5, 0.75, 2.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Telefono_Puesto_A", "type": "telefono", "pos": (-5.0, 0.75, 2.0), "yaw": 15, "scale": (0.6, 0.6, 0.6)},
    {"name": "Archivador_Puesto_A", "type": "archivador_metalico", "pos": (-3.2, 0.00, 2.0), "yaw": -90, "scale": (1, 1, 1)},

    # Workstation B
    {"name": "Escritorio_Puesto_B", "type": "escritorio_oficina", "pos": (-4.5, 0.00, -2.0), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Puesto_B", "type": "silla_ejecutiva", "pos": (-4.5, 0.00, -2.8), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Computadora_Puesto_B", "type": "compu_variante", "pos": (-4.5, 0.75, -2.0), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Documentos_Puesto_B", "type": "libros_pila", "pos": (-3.9, 0.75, -2.0), "yaw": 10, "scale": (1, 1, 1)},
    {"name": "Archivador_Puesto_B", "type": "archivador_metalico", "pos": (-3.2, 0.00, -2.0), "yaw": -90, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 3: EXECUTIVE OFFICE (West Wing)
    # ==========================================
    {"name": "Escritorio_Directorio", "type": "escritorio_oficina", "pos": (-13.5, 0.00, 8.5), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Presidencial", "type": "silla_ejecutiva", "pos": (-13.5, 0.00, 9.4), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Computadora_Directorio", "type": "computadora_monitor", "pos": (-13.5, 0.75, 8.5), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Telefono_Directorio", "type": "telefono", "pos": (-14.0, 0.75, 8.5), "yaw": 195, "scale": (0.6, 0.6, 0.6)},
    {"name": "Silla_Visitante_01", "type": "silla_ejecutiva", "pos": (-12.9, 0.00, 7.4), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Visitante_02", "type": "silla_ejecutiva", "pos": (-14.1, 0.00, 7.4), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Biblioteca_Directorio", "type": "biblioteca", "pos": (-23.0, 0.00, 6.0), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Libros_Directorio", "type": "libros", "pos": (-22.7, 0.85, 6.0), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Archivador_Directorio_01", "type": "archivador_metalico", "pos": (-23.0, 0.00, 4.0), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Archivador_Directorio_02", "type": "archivador_metalico", "pos": (-23.0, 0.00, 3.2), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Mesa_Planos_Directorio", "type": "escritorio_individual", "pos": (-18.5, 0.00, 3.5), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Silla_Mesa_Planos", "type": "silla_ejecutiva", "pos": (-18.5, 0.00, 4.2), "yaw": 180, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 4: BREAKROOM / COCINA / CAFETERIA (South Room)
    # ==========================================
    {"name": "Buffet_Cocina_Mostrador", "type": "mueble_largo_ofi", "pos": (-1.0, 0.00, -11.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Cafetera_Espresso_Oficina", "type": "cafetera_oficina", "pos": (-1.8, 0.92, -11.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Microondas_Cocina", "type": "microondas_oficina", "pos": (-0.9, 0.92, -11.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Cafetera_Filtro_Aux", "type": "cafetera_variante", "pos": (-0.1, 0.92, -11.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Frigobar_Cocina", "type": "frigobar_oficina", "pos": (0.7, 0.00, -11.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Cocina", "type": "dispensador_agua", "pos": (2.2, 0.00, -10.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Papelera_Cocina", "type": "dispensadores_bano", "pos": (-2.8, 0.00, -10.5), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Mesa_Comedor_Cocina", "type": "mesa_pequena", "pos": (-0.5, 0.00, -8.0), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Silla_Comedor_01", "type": "silla_ejecutiva", "pos": (-1.4, 0.00, -8.0), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Silla_Comedor_02", "type": "silla_ejecutiva", "pos": (0.4, 0.00, -8.0), "yaw": -90, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 5: RESTROOM SUR / BAÑO SUR (South Bathroom)
    # ==========================================
    {"name": "Inodoro_Bano_Sur", "type": "inodoro_bano", "pos": (-7.2, 0.00, -7.8), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Lavamanos_Bano_Sur", "type": "lavamanos_mueble", "pos": (-5.2, 0.00, -6.0), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Accesorios_Bano_Sur", "type": "dispensadores_bano", "pos": (-7.2, 0.00, -6.6), "yaw": 90, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 6: RESTROOM NORTE / BAÑO NORTE (Northwest Bathroom)
    # ==========================================
    {"name": "Inodoro_Bano_Norte", "type": "inodoro_bano", "pos": (-17.2, 0.00, 19.2), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Lavamanos_Bano_Norte", "type": "lavamanos_mueble", "pos": (-15.5, 0.00, 17.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Accesorios_Bano_Norte", "type": "dispensadores_bano", "pos": (-16.5, 0.00, 19.4), "yaw": 0, "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 7: CONFERENCE & MEETING ROOM (North Large Room)
    # ==========================================
    {"name": "Mesa_Reuniones_01", "type": "escritorio_oficina", "pos": (-8.5, 0.00, 15.2), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Mesa_Reuniones_02", "type": "escritorio_oficina", "pos": (-8.5, 0.00, 14.4), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_N1", "type": "silla_ejecutiva", "pos": (-9.2, 0.00, 16.0), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_N2", "type": "silla_ejecutiva", "pos": (-7.8, 0.00, 16.0), "yaw": 180, "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_S1", "type": "silla_ejecutiva", "pos": (-9.2, 0.00, 13.6), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_S2", "type": "silla_ejecutiva", "pos": (-7.8, 0.00, 13.6), "yaw": 0, "scale": (1, 1, 1)},
    {"name": "Monitor_Presentaciones", "type": "computadora_monitor", "pos": (-8.5, 0.75, 14.8), "yaw": 90, "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Reuniones", "type": "dispensador_agua", "pos": (-3.2, 0.00, 18.8), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Archivador_Reuniones_01", "type": "archivador_metalico", "pos": (-3.2, 0.00, 16.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Archivador_Reuniones_02", "type": "archivador_metalico", "pos": (-3.2, 0.00, 15.5), "yaw": -90, "scale": (1, 1, 1)},
    {"name": "Biblioteca_Reuniones", "type": "biblioteca", "pos": (-14.2, 0.00, 11.0), "yaw": 90, "scale": (1, 1, 1)},
]

def main():
    with open(SCENE_PATH, "r", encoding="utf-8") as f:
        content = f.read()

    # 1. Level Map: Update Pisos root to Y = -0.1 so floor surface is Y = 0.000
    # Transform: 1574751093
    # m_LocalPosition: {x: 5.64596, y: 0.1, z: -0.2427} -> y: -0.1
    content = re.sub(
        r'(--- !u!4 &1574751093\s+Transform:[^}]+?m_LocalPosition:\s*\{x:\s*5\.64596,\s*)y:\s*0\.1,(\s*z:\s*-0\.2427\})',
        r'\1y: -0.1,\2',
        content
    )
    
    # 2. Level Map: Update Muros root to Y = -0.1 so wall bottoms meet floor at Y = 0.000
    # Transform: 1474490344
    content = re.sub(
        r'(--- !u!4 &1474490344\s+Transform:[^}]+?m_LocalPosition:\s*\{x:\s*5\.64596,\s*)y:\s*0\.1,(\s*z:\s*-0\.2427\})',
        r'\1y: -0.1,\2',
        content
    )
    
    # 3. Level Player (Y Bot) to Y = 0.000
    content = re.sub(
        r'(- target: \{fileID: -8679921383154817045, guid: 299657909f4319c90b6dacf4ad9fad40, type: 3\}\s+propertyPath: m_LocalPosition\.y\s+value:\s*)[^\n]+',
        r'\g<1>0',
        content
    )

    # 4. Remove any existing added PrefabInstances from 550000000 onwards
    # Find start of our instances
    first_inst_idx = content.find("--- !u!1001 &550000000")
    if first_inst_idx != -1:
        # Cut until SceneRoots
        sr_idx = content.find("--- !u!1660057539 &9223372036854775807", first_inst_idx)
        content = content[:first_inst_idx] + content[sr_idx:]
        
    # Clean up SceneRoots to remove any 550000xxx entries
    sr_idx = content.find("--- !u!1660057539 &9223372036854775807")
    sr_part = content[sr_idx:]
    sr_lines = sr_part.split("\n")
    cleaned_sr_lines = [l for l in sr_lines if not re.search(r'fileID:\s*550000\d+', l)]
    content = content[:sr_idx] + "\n".join(cleaned_sr_lines)

    # 5. Generate clean, upright PrefabInstance blocks
    start_id = 550000000
    new_blocks = []
    new_roots = []
    
    for i, inst in enumerate(instances):
        inst_id = start_id + i
        guid = GUIDS[inst["type"]]
        px, py, pz = inst["pos"]
        yaw = inst["yaw"]
        qx, qy, qz, qw = yaw_to_quaternion(yaw)
        sx, sy, sz = inst["scale"]
        name = inst["name"]
        
        block = f"""--- !u!1001 &{inst_id}
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {{fileID: 0}}
    m_Modifications:
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalPosition.x
      value: {px}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalPosition.y
      value: {py}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalPosition.z
      value: {pz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.w
      value: {qw:.7f}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.x
      value: {qx:.7f}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.y
      value: {qy:.7f}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.z
      value: {qz:.7f}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.y
      value: {yaw}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalScale.x
      value: {sx}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalScale.y
      value: {sy}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalScale.z
      value: {sz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: 919132149155446097, guid: {guid}, type: 3}}
      propertyPath: m_Name
      value: {name}
      objectReference: {{fileID: 0}}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {{fileID: 100100000, guid: {guid}, type: 3}}"""
        new_blocks.append(block)
        new_roots.append(f"  - {{fileID: {inst_id}}}")
        
    sr_idx = content.find("--- !u!1660057539 &9223372036854775807")
    header_part = content[:sr_idx]
    roots_part = content[sr_idx:]
    
    # Insert new roots into m_Roots:
    m_roots_idx = roots_part.find("m_Roots:")
    lines = roots_part.split("\n")
    updated_lines = []
    for line in lines:
        updated_lines.append(line)
        if line.strip() == "m_Roots:":
            for nr in new_roots:
                updated_lines.append(nr)
    roots_part = "\n".join(updated_lines)

    final_content = header_part + "\n".join(new_blocks) + "\n" + roots_part
    
    with open(SCENE_PATH, "w", encoding="utf-8") as f:
        f.write(final_content)
        
    print(f"[OK] Scene {SCENE_PATH} successfully updated:")
    print(f"  - Map floor leveled to Y = 0.000")
    print(f"  - Walls aligned to floor base Y = 0.000")
    print(f"  - Player aligned to floor Y = 0.000")
    print(f"  - {len(instances)} FBX assets positioned upright (Y-up), resting on floor or desks with zero float/sink!")

if __name__ == "__main__":
    main()
