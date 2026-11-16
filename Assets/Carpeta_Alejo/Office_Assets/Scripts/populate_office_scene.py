import os
import re
import math

SCENE_PATH = "Assets/Carpeta_Alejo/Mapa.unity"

# FBX GUIDs from Assets/Carpeta_Alejo/Office_Assets/FBX/*.meta
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
    "silla_plastico": "7662678e01c21132195941de5ef859dc",
    "sillon_individual": "3604f2492f26447ecadbc526212c4d27",
    "telefono": "56af9a425359a625bb86ca932e678982",
    "biblioteca": "00fe222a400db6ddd8cb3c47a052387a",
    "cafetera_variante": "653b9262168b93a1e8e07658de75fb1e",
    "compu_variante": "9a8002d18a59f3f2788964996df40f8f",
    "libros_pila": "32ab2c91db84228e5a0c45c697e88aee",
    "libros": "c2d61e69cbffb064cb3054dfd00b87b8",
}

def euler_to_quaternion(yaw_y, pitch_x=0.0, roll_z=0.0):
    rad_y = math.radians(yaw_y) * 0.5
    rad_x = math.radians(pitch_x) * 0.5
    rad_z = math.radians(roll_z) * 0.5
    cy = math.cos(rad_y)
    sy = math.sin(rad_y)
    cx = math.cos(rad_x)
    sx = math.sin(rad_x)
    cz = math.cos(rad_z)
    sz = math.sin(rad_z)
    qw = cx * cy * cz + sx * sy * sz
    qx = sx * cy * cz - cx * sy * sz
    qy = cx * sy * cz + sx * cy * sz
    qz = cx * cy * sz - sx * sy * cz
    return qx, qy, qz, qw

# 56 Carefully Designed Placements Across the Office Map
instances = [
    # ==========================================
    # ZONE 1: RECEPTION & WAITING LOUNGE (East Central Hall)
    # ==========================================
    {"name": "Sillon_Espera_01", "type": "sillon_individual", "pos": (3.5, 0.20, 0.8), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Sillon_Espera_02", "type": "sillon_individual", "pos": (3.5, 0.20, -0.8), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Mesa_Espera_01", "type": "mesa_pequena", "pos": (3.5, 0.20, 0.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Revistas_Espera_01", "type": "libros", "pos": (3.5, 1.13, 0.0), "rot": (0, 25, 0), "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Espera", "type": "dispensador_agua", "pos": (6.8, 0.20, 2.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Biblioteca_Recepcion", "type": "biblioteca", "pos": (7.3, 0.20, -1.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Libros_Pila_Recepcion", "type": "libros_pila", "pos": (7.1, 1.05, -1.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Auxiliar_Recepcion", "type": "silla_plastico", "pos": (6.5, 0.20, -3.0), "rot": (0, -45, 0), "scale": (100, 100, 100)},

    # ==========================================
    # ZONE 2: WORKSTATION AREA (West Central Hall)
    # ==========================================
    # Workstation A
    {"name": "Escritorio_Puesto_A", "type": "escritorio_oficina", "pos": (-4.5, 0.20, 2.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Puesto_A", "type": "silla_ejecutiva", "pos": (-4.5, 0.20, 2.8), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Computadora_Puesto_A", "type": "computadora_monitor", "pos": (-4.5, 0.95, 2.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Telefono_Puesto_A", "type": "telefono", "pos": (-5.0, 0.95, 2.0), "rot": (0, 15, 0), "scale": (0.6, 0.6, 0.6)},
    {"name": "Archivador_Puesto_A", "type": "archivador_metalico", "pos": (-3.2, 0.20, 2.0), "rot": (0, -90, 0), "scale": (1, 1, 1)},

    # Workstation B
    {"name": "Escritorio_Puesto_B", "type": "escritorio_oficina", "pos": (-4.5, 0.20, -2.0), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Puesto_B", "type": "silla_ejecutiva", "pos": (-4.5, 0.20, -2.8), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Computadora_Puesto_B", "type": "compu_variante", "pos": (-4.5, 0.95, -2.0), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Documentos_Puesto_B", "type": "libros_pila", "pos": (-3.9, 0.95, -2.0), "rot": (0, 10, 0), "scale": (1, 1, 1)},
    {"name": "Archivador_Puesto_B", "type": "archivador_metalico", "pos": (-3.2, 0.20, -2.0), "rot": (0, -90, 0), "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 3: EXECUTIVE OFFICE (West Wing)
    # ==========================================
    {"name": "Escritorio_Directorio", "type": "escritorio_oficina", "pos": (-13.5, 0.20, 8.5), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Presidencial", "type": "silla_ejecutiva", "pos": (-13.5, 0.20, 9.4), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Computadora_Directorio", "type": "computadora_monitor", "pos": (-13.5, 0.95, 8.5), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Telefono_Directorio", "type": "telefono", "pos": (-14.0, 0.95, 8.5), "rot": (0, 195, 0), "scale": (0.6, 0.6, 0.6)},
    {"name": "Silla_Visitante_01", "type": "silla_plastico", "pos": (-12.9, 0.20, 7.4), "rot": (0, 180, 0), "scale": (100, 100, 100)},
    {"name": "Silla_Visitante_02", "type": "silla_plastico", "pos": (-14.1, 0.20, 7.4), "rot": (0, 180, 0), "scale": (100, 100, 100)},
    {"name": "Biblioteca_Directorio", "type": "biblioteca", "pos": (-23.0, 0.20, 6.0), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Libros_Directorio", "type": "libros", "pos": (-22.7, 1.05, 6.0), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Archivador_Directorio_01", "type": "archivador_metalico", "pos": (-23.0, 0.20, 4.0), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Archivador_Directorio_02", "type": "archivador_metalico", "pos": (-23.0, 0.20, 3.2), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Mesa_Planos_Directorio", "type": "escritorio_individual", "pos": (-18.5, 0.20, 3.5), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Mesa_Planos", "type": "silla_ejecutiva", "pos": (-18.5, 0.20, 4.2), "rot": (0, 180, 0), "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 4: BREAKROOM / COCINA / CAFETERIA (South Room)
    # ==========================================
    {"name": "Buffet_Cocina_Mostrador", "type": "mueble_largo_ofi", "pos": (-1.0, 0.20, -11.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Cafetera_Espresso_Oficina", "type": "cafetera_oficina", "pos": (-1.8, 1.12, -11.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Microondas_Cocina", "type": "microondas_oficina", "pos": (-0.9, 1.12, -11.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Cafetera_Filtro_Aux", "type": "cafetera_variante", "pos": (-0.1, 1.12, -11.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Frigobar_Cocina", "type": "frigobar_oficina", "pos": (0.7, 0.20, -11.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Cocina", "type": "dispensador_agua", "pos": (2.2, 0.20, -10.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Papelera_Cocina", "type": "dispensadores_bano", "pos": (-2.8, 0.20, -10.5), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Mesa_Comedor_Cocina", "type": "mesa_pequena", "pos": (-0.5, 0.20, -8.0), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Comedor_01", "type": "silla_plastico", "pos": (-1.4, 0.20, -8.0), "rot": (0, 90, 0), "scale": (100, 100, 100)},
    {"name": "Silla_Comedor_02", "type": "silla_plastico", "pos": (0.4, 0.20, -8.0), "rot": (0, -90, 0), "scale": (100, 100, 100)},

    # ==========================================
    # ZONE 5: RESTROOM SUR / BAÑO SUR (South Bathroom)
    # ==========================================
    {"name": "Inodoro_Bano_Sur", "type": "inodoro_bano", "pos": (-7.2, 0.20, -7.8), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Lavamanos_Bano_Sur", "type": "lavamanos_mueble", "pos": (-5.2, 0.20, -6.0), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Accesorios_Bano_Sur", "type": "dispensadores_bano", "pos": (-7.2, 0.20, -6.6), "rot": (0, 90, 0), "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 6: RESTROOM NORTE / BAÑO NORTE (Northwest Bathroom)
    # ==========================================
    {"name": "Inodoro_Bano_Norte", "type": "inodoro_bano", "pos": (-17.2, 0.20, 19.2), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Lavamanos_Bano_Norte", "type": "lavamanos_mueble", "pos": (-15.5, 0.20, 17.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Accesorios_Bano_Norte", "type": "dispensadores_bano", "pos": (-16.5, 0.20, 19.4), "rot": (0, 0, 0), "scale": (1, 1, 1)},

    # ==========================================
    # ZONE 7: CONFERENCE & MEETING ROOM (North Large Room)
    # ==========================================
    {"name": "Mesa_Reuniones_01", "type": "escritorio_oficina", "pos": (-8.5, 0.20, 15.2), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Mesa_Reuniones_02", "type": "escritorio_oficina", "pos": (-8.5, 0.20, 14.4), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_N1", "type": "silla_ejecutiva", "pos": (-9.2, 0.20, 16.0), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_N2", "type": "silla_ejecutiva", "pos": (-7.8, 0.20, 16.0), "rot": (0, 180, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_S1", "type": "silla_ejecutiva", "pos": (-9.2, 0.20, 13.6), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Silla_Conferencia_S2", "type": "silla_ejecutiva", "pos": (-7.8, 0.20, 13.6), "rot": (0, 0, 0), "scale": (1, 1, 1)},
    {"name": "Monitor_Presentaciones", "type": "computadora_monitor", "pos": (-8.5, 0.95, 14.8), "rot": (0, 90, 0), "scale": (1, 1, 1)},
    {"name": "Dispensador_Agua_Reuniones", "type": "dispensador_agua", "pos": (-3.2, 0.20, 18.8), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Archivador_Reuniones_01", "type": "archivador_metalico", "pos": (-3.2, 0.20, 16.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Archivador_Reuniones_02", "type": "archivador_metalico", "pos": (-3.2, 0.20, 15.5), "rot": (0, -90, 0), "scale": (1, 1, 1)},
    {"name": "Biblioteca_Reuniones", "type": "biblioteca", "pos": (-14.2, 0.20, 11.0), "rot": (0, 90, 0), "scale": (1, 1, 1)},
]

def generate_yaml():
    with open(SCENE_PATH, "r", encoding="utf-8") as f:
        content = f.read()

    # Base ID for new instances
    start_id = 550000000
    
    new_blocks = []
    new_roots = []
    
    for i, inst in enumerate(instances):
        inst_id = start_id + i
        guid = GUIDS[inst["type"]]
        px, py, pz = inst["pos"]
        ey, ep, er = inst["rot"][1], inst["rot"][0], inst["rot"][2]
        qx, qy, qz, qw = euler_to_quaternion(ey, ep, er)
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
      value: {qw}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.x
      value: {qx}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.y
      value: {qy}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalRotation.z
      value: {qz}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.x
      value: {ep}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.y
      value: {ey}
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.z
      value: {er}
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
        
    # Split content right before SceneRoots
    sr_idx = content.rfind("--- !u!1660057539 &9223372036854775807")
    if sr_idx == -1:
        sr_idx = content.rfind("SceneRoots:")
        header_part = content[:sr_idx]
        roots_part = content[sr_idx:]
    else:
        header_part = content[:sr_idx]
        roots_part = content[sr_idx:]
        
    # Add new roots into SceneRoots
    # Find m_Roots: in roots_part
    m_roots_idx = roots_part.find("m_Roots:")
    if m_roots_idx != -1:
        lines = roots_part.split("\n")
        updated_lines = []
        for line in lines:
            updated_lines.append(line)
            if line.strip() == "m_Roots:":
                for nr in new_roots:
                    updated_lines.append(nr)
        roots_part = "\n".join(updated_lines)

    # Combine everything
    all_new_blocks_text = "\n".join(new_blocks) + "\n"
    final_content = header_part + all_new_blocks_text + roots_part
    
    with open(SCENE_PATH, "w", encoding="utf-8") as f:
        f.write(final_content)
        
    print(f"Successfully added {len(instances)} FBX office assets into {SCENE_PATH}!")

if __name__ == "__main__":
    generate_yaml()
