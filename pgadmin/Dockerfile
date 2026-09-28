FROM dpage/pgadmin4:latest

COPY servers.json /pgadmin4/servers.json

COPY pgpass /tmp/pgpass

USER root

RUN mkdir -p /var/lib/pgadmin/storage \
    && cp /tmp/pgpass /pgpass \
    && chmod 600 /pgpass

USER 5050