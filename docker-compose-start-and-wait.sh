#!/bin/bash
echo "Starting docker containers..."
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d --force-recreate

if [ $? -ne 0 ]; then
	echo "Unable to start containers"
	exit 1
fi

declare -a logchecks=(
	
	"sqlserver.*SQL Server is now ready for client connections"
	)

declare status=-1
until [ $status -eq 0 ]; do
    status=0
	for check in "${logchecks[@]}"
	do
		docker compose logs | grep -e "$check"
		status=$(($status + $?))
	done
	if [ $status -ne 0 ]; then
		>&2 echo "Service ready checks have failed - sleeping"
		sleep 10
	fi
done

>&2 echo "Service ready checks succeeded!"