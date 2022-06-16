#sleep 30s
#
#/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d master -i create-db.sql
#/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d master -i stored-procedures.sql


#Creating databases

for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d master -i create-db.sql
    if [ $? -eq 0 ]
    then
        echo "create-db.sql completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done

#Setup EventManagement database

for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d EventManagement -i create-tables.sql
    if [ $? -eq 0 ]
    then
        echo "stored-procedures.sql for EventManagement completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done

for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d EventManagement -i create-triggers.sql
    if [ $? -eq 0 ]
    then
        echo "create-triggers.sql for EventManagement completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done

for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d EventManagement -i stored-procedures.sql
    if [ $? -eq 0 ]
    then
        echo "stored-procedures.sql for EventManagement completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done



#Setup EventManagementTests database

for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d EventManagementTests -i create-tables.sql
    if [ $? -eq 0 ]
    then
        echo "stored-procedures.sql for EventManagement completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done



for i in {1..30};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password_123BD -d EventManagementTests -i stored-procedures.sql
    if [ $? -eq 0 ]
    then
        echo "stored-procedures.sql for EventManagementTests completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done


