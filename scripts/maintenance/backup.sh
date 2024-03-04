#!/bin/bash

# Script de backup automatizado
# Uso: ./backup.sh [directorio_destino]

# Configuración
BACKUP_DIR="/backup/productos"
DB_CONTAINER="productos-sqlserver"
DB_USER="sa"
DB_PASSWORD="Password123!"
DB_NAME="ProductosDB"
RETENTION_DAYS=7

# Crear directorio de backup si no existe
mkdir -p "$BACKUP_DIR"

# Timestamp para el nombre del archivo
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Backup de la base de datos
echo "Iniciando backup de la base de datos..."
docker exec $DB_CONTAINER /opt/mssql-tools/bin/sqlcmd \
    -U $DB_USER \
    -P $DB_PASSWORD \
    -Q "BACKUP DATABASE [$DB_NAME] TO DISK = N'/var/opt/mssql/backup/${DB_NAME}_${TIMESTAMP}.bak' WITH NOFORMAT, NOINIT, SKIP, NOREWIND, NOUNLOAD, STATS = 10"

# Backup de los logs
echo "Copiando logs de auditoría..."
cp -r /app/logs/audit/* "$BACKUP_DIR/logs_${TIMESTAMP}/"

# Backup de configuraciones
echo "Respaldando archivos de configuración..."
cp /app/appsettings*.json "$BACKUP_DIR/config_${TIMESTAMP}/"

# Comprimir todos los backups del día
tar -czf "$BACKUP_DIR/backup_${TIMESTAMP}.tar.gz" \
    "$BACKUP_DIR/${DB_NAME}_${TIMESTAMP}.bak" \
    "$BACKUP_DIR/logs_${TIMESTAMP}" \
    "$BACKUP_DIR/config_${TIMESTAMP}"

# Limpiar backups antiguos
find "$BACKUP_DIR" -name "backup_*.tar.gz" -mtime +$RETENTION_DAYS -delete

echo "Backup completado: backup_${TIMESTAMP}.tar.gz"