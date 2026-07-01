#!/bin/sh
set -e
NGINX_PROXY_URL="${NGINX_PROXY_URL:-http://backend:8080}"
NGINX_PROXY_URL=$(echo "${NGINX_PROXY_URL}" | sed 's:/*$::')

echo "Substituting backend URL: ${NGINX_PROXY_URL}"
export NGINX_PROXY_URL
envsubst '${NGINX_PROXY_URL}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

exec "$@"
