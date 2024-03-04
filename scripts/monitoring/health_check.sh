#!/bin/bash

# Configuración
API_URL="http://localhost:5001/health"
DB_CONTAINER="productos-sqlserver"
SLACK_WEBHOOK_URL="${SLACK_WEBHOOK_URL:-}"
LOG_FILE="/var/log/productos/health_check.log"

# Crear directorio de logs si no existe
mkdir -p "$(dirname "$LOG_FILE")"

log_message() {
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo "[$timestamp] $1" | tee -a "$LOG_FILE"
}

check_api_health() {
    local response=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL")
    if [ "$response" = "200" ]; then
        log_message "API Health Check: OK"
        return 0
    else
        log_message "API Health Check: ERROR - Status code: $response"
        return 1
    fi
}

check_db_health() {
    local db_status=$(docker exec $DB_CONTAINER /opt/mssql-tools/bin/sqlcmd \
        -U sa -P "Password123!" \
        -Q "SELECT 1" -h -1 2>/dev/null)
    
    if [ $? -eq 0 ]; then
        log_message "Database Health Check: OK"
        return 0
    else
        log_message "Database Health Check: ERROR"
        return 1
    fi
}

check_disk_space() {
    local threshold=90
    local usage=$(df -h / | tail -1 | awk '{print $5}' | cut -d'%' -f1)
    
    if [ "$usage" -lt "$threshold" ]; then
        log_message "Disk Space Check: OK ($usage%)"
        return 0
    else
        log_message "Disk Space Check: WARNING - High usage ($usage%)"
        return 1
    fi
}

send_alert() {
    if [ -n "$SLACK_WEBHOOK_URL" ]; then
        curl -s -X POST -H 'Content-type: application/json' \
            --data "{\"text\":\"🚨 Alert: $1\"}" \
            "$SLACK_WEBHOOK_URL"
    fi
    log_message "Alert sent: $1"
}

# Ejecutar verificaciones
errors=0

check_api_health || ((errors++))
check_db_health || ((errors++))
check_disk_space || ((errors++))

# Enviar alerta si hay errores
if [ $errors -gt 0 ]; then
    send_alert "Se detectaron $errors problemas en el sistema. Revisar $LOG_FILE"
    exit 1
fi

exit 0