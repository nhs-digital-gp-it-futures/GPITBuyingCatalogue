#/bin/bash

docker-compose stop &&
docker-compose rm -f &&
docker-compose -f "tests/NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests/Pipeline/docker-compose.yml" up --scale chrome=4 -d
