#!/bin/bash

# Script de backup automatizado
# Uso: ./backup.sh [directorio_destino]

# Configuración
BACKUP_DIR=${1:-"/backup"}
DATE=$(date +%Y%m%d_%H%M%S)
DB_CONTAINER="prueba-sqlserver-1"
DB_USER="sa"
DB_PASSWORD="Password123!"
DB_NAME="ProductosDB"

# Crear directorio de backup si no existe
mkdir -p "$BACKUP_DIR"

# Backup de la base de datos
echo "Iniciando backup de base de datos..."
docker exec $DB_CONTAINER /opt/mssql-tools/bin/sqlcmd \
    -S localhost -U $DB_USER -P $DB_PASSWORD \
    -Q "BACKUP DATABASE $DB_NAME TO DISK = '/var/opt/mssql/backup/${DB_NAME}_${DATE}.bak' WITH FORMAT"

# Backup de configuraciones
echo "Realizando backup de configuraciones..."
tar -czf "$BACKUP_DIR/config_${DATE}.tar.gz" \
    --exclude='.git' \
    --exclude='node_modules' \
    --exclude='bin' \
    --exclude='obj' \
    ../ProductosAPI.Core/appsettings*.json \
    ../nginx.conf \
    ../docker-compose*.yml

# Limpiar backups antiguos (mantener últimos 7 días)
echo "Limpiando backups antiguos..."
find "$BACKUP_DIR" -name "*.bak" -mtime +7 -delete
find "$BACKUP_DIR" -name "*.tar.gz" -mtime +7 -delete

echo "Backup completado: $(date)"