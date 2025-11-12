# Reorganización del Frontend — pautas y puerto fijo

Fecha: 2025-11-12

Resumen rápido
- El frontend fue reorganizado en una estructura por características bajo `src/app`.
- Se separó el componente `admin-dashboard` en 3 archivos (TS / HTML / SCSS).
- El servidor de desarrollo se fijó para ejecutarse siempre en el puerto 4200 (ver `package.json` -> `scripts.start`).

Puerto y cómo iniciar
- Puerto fijado: 4200
- Iniciar servidor de desarrollo (desde la carpeta `Frontend`):

```powershell
cd "C:\Users\jsola\Desktop\ADSO\EstancoPro\Frontend"
npm start
```

Notas: `npm start` ejecuta `ng serve --port 4200`. Si quieres servir en otra máquina o exponer externamente, usa `ng serve --host 0.0.0.0 --port 4200`.

Arquitectura propuesta y cambios realizados
- Convención principal (ya aplicada parcialmente):
  - `src/app/core/` — servicios, guards, interceptors, modelos principales.
  - `src/app/shared/` — componentes reutilizables, pipes, directivas, config centralizada (ej. `menu-sections.ts`).
  - `src/app/features/` — features por dominio. Ejemplo: `features/admin/` con subcarpetas por entidad:
    - `features/admin/dashboard/` (componentes de dashboard)
    - `features/admin/users/`
    - `features/admin/products/`
    - `features/admin/suppliers/`
    - `features/admin/categories/`
    - `features/admin/unit-measures/`
    - `features/admin/roles/`

Qué hice exactamente
- Creé: `src/app/features/admin/dashboard/admin-dashboard.component.html` y `.scss` y adapté el `.ts` para usar `templateUrl`/`styleUrls`.
- Moví/creé componentes admin en subcarpetas `users/`, `roles/`, `categories/`, `suppliers/`, `unit-measures/`.
- Actualicé `src/app/app.routes.ts` para apuntar a las nuevas rutas de import.
- Fijé el puerto en `Frontend/package.json` (`start` script) a `--port 4200`.

Archivos duplicados / candidatos seguros a eliminar
Antes de eliminar, asegúrate de commitear o crear un branch/backup. Los archivos listados abajo parecen duplicados (se movieron/copied a subcarpetas). Si confirmas, puedo borrarlos automáticamente.

- `Frontend/src/app/features/admin/admin-dashboard.component.ts`  (migrado a `features/admin/dashboard/`)
- `Frontend/src/app/features/admin/admin-users.component.ts`      (duplicado — ver `features/admin/users/`)
- `Frontend/src/app/features/admin/admin-users-clean.component.ts`
- `Frontend/src/app/features/admin/admin-users-compact.component.ts`
- `Frontend/src/app/features/admin/admin-roles.component.ts`      (duplicado — ver `features/admin/roles/`)
- `Frontend/src/app/features/admin/admin-roles-compact.component.ts`
- `Frontend/src/app/features/admin/admin-categories-compact.component.ts`
- `Frontend/src/app/features/admin/admin-suppliers-compact.component.ts`
- `Frontend/src/app/features/admin/admin-unit-measures-compact.component.ts`

Cómo eliminar de forma segura
1. Crear un commit o branch de respaldo:

```powershell
cd "C:\Users\jsola\Desktop\ADSO\EstancoPro"
git checkout -b chore/frontend-reorg-backup
git add -A
git commit -m "backup before frontend reorganization cleanup"
```

2. Para borrar los archivos listados (ejemplo):

```powershell
cd "C:\Users\jsola\Desktop\ADSO\EstancoPro\Frontend"
git rm src/app/features/admin/admin-users.component.ts
git rm src/app/features/admin/admin-users-clean.component.ts
git rm src/app/features/admin/admin-users-compact.component.ts
git rm src/app/features/admin/admin-roles.component.ts
git rm src/app/features/admin/admin-roles-compact.component.ts
git rm src/app/features/admin/admin-categories-compact.component.ts
git rm src/app/features/admin/admin-suppliers-compact.component.ts
git rm src/app/features/admin/admin-unit-measures-compact.component.ts
git commit -m "chore: remove legacy/duplicated admin files after reorg"
```

3. Ejecutar build y probar dev-server:

```powershell
npm run build
npm start
# abrir http://localhost:4200
```

Verificaciones y acciones posteriores
- Revisa la aplicación en `http://localhost:4200` y recarga duro (Ctrl+F5) si el navegador muestra contenido viejo.
- Si utilizas Service Workers o PWA, asegúrate de desactivar o registrar/unregister worker en el navegador devtools → Application → Service Workers.

Limpieza opcional (recomendado)
- Unificar y crear `index.ts` (barrels) en `shared/components` y `features/*` para simplificar imports.
- Ejecutar linter y arreglar warnings (por ejemplo las advertencias Sass deprecadas y budgets de CSS por componente).

Si quieres, procedo a:
1) Eliminar los archivos duplicados listados (haré backup commit antes).  
2) Consolidar imports (crear barrels `index.ts`) para `shared` y `features/admin`.
3) Limpiar advertencias NG8113 eliminando imports no usados u organizando `standalone` en componentes compartidos.

Indica qué quieres que haga automáticamente ahora (por ejemplo: "Eliminar duplicados ahora") o si prefieres revisar la lista antes de borrar.

---
Archivo generado automáticamente por la reorganización realizada por el equipo el 2025-11-12.
