FROM node:18-alpine as build-stage

WORKDIR /app

COPY tracknsave.client/package*.json ./

RUN npm ci

COPY tracknsave.client/ .

RUN npm run build

FROM nginx:alpine as production-stage

COPY --from=build-stage /app/dist /usr/share/nginx/html

COPY tracknsave.client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]