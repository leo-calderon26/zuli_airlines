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

# Stage de producción con nginx
FROM nginx:alpine AS runtime

# Copiar configuración de nginx
COPY zuli_frontend/nginx.conf /etc/nginx/conf.d/default.conf

# Copiar archivos compilados
COPY --from=build /app/dist /usr/share/nginx/html

# Exponer puerto
EXPOSE 80

# Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
