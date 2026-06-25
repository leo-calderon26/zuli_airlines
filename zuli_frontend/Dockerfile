# Frontend Vue 3 Dockerfile
FROM node:20-alpine AS build
WORKDIR /app

# Instalar pnpm
RUN npm install -g pnpm

# Copiar archivos de dependencias
COPY zuli_frontend/package.json zuli_frontend/pnpm-lock.yaml ./

# Instalar dependencias
RUN pnpm install --frozen-lockfile

# Copiar todo el código fuente
COPY zuli_frontend/ .

# Compilar para producción
RUN pnpm run build

# Verificar que el build generó los archivos estáticos
RUN test -d /app/dist && test -f /app/dist/index.html && ls -la /app/dist

# Stage de producción con nginx
FROM nginx:alpine AS runtime

# Instalar envsubst para substituir variables de entorno
RUN apk add --no-cache gettext

# Copiar archivos compilados
COPY --from=build /app/dist /usr/share/nginx/html

# Copiar script de entrypoint
COPY zuli_frontend/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Copiar plantilla de nginx
COPY zuli_frontend/nginx.conf /etc/nginx/conf.d/default.conf.template

# Exponer puerto
EXPOSE 80

# Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://127.0.0.1/ || exit 1

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
