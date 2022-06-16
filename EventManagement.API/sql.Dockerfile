FROM mcr.microsoft.com/mssql/server 

USER root

ARG PROJECT_DIR=/tmp/em_project
RUN mkdir -p $PROJECT_DIR
WORKDIR $PROJECT_DIR
ENV SA_PASSWORD=Password_123BD
ENV ACCEPT_EULA=Y
COPY create-db.sql ./
COPY create-tables.sql ./
COPY create-triggers.sql ./
COPY stored-procedures.sql ./
COPY entrypoint.sh ./
COPY setup.sh ./

RUN chmod +x setup.sh
CMD ["/bin/bash", "entrypoint.sh"]
