#!/bin/bash

# Check if a query was provided
if [ -z "$1" ]; then
  echo "Uso: ./test_cli.sh \"tu consulta en lenguaje natural\""
  echo "Ejemplo: ./test_cli.sh \"listar archivos ocultos\""
  exit 1
fi

# API Endpoint (HTTPS port from launchSettings.json)
URL="https://localhost:7277/suggest"

# JSON Payload
# We need to escape quotes in the input
INPUT=$(echo "$1" | sed 's/"/\\"/g')
CURRENT_DIR=$(pwd)

# Call API
echo "Enviando consulta: \"$1\"..."
echo "Contexto: $CURRENT_DIR"
echo "---------------------------------------------------"

# Use curl with -k to ignore self-signed certificate issues in development
curl -s -k -X POST "$URL" \
     -H "Content-Type: application/json" \
     -d "{
           \"naturalLanguageInput\": \"$INPUT\",
           \"context\": {
             \"os\": \"Linux\",
             \"shell\": \"Bash\",
             \"currentDirectory\": \"$CURRENT_DIR\"
           }
         }"

echo "" # Newline
echo "---------------------------------------------------"
