FROM node:24-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY angular.json tsconfig.* ./
COPY public public
COPY src src

RUN npm run build



FROM nginx:1.29-alpine

COPY nginx.conf /etc/nginx/nginx.conf

WORKDIR /app
COPY --chown=nginx:nginx --from=build /app/dist/movie-screening-frontend/browser www/
