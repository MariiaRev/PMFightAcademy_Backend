docker build -t registry.digitalocean.com/pm-fight-backend/pmfightacademyclient:0.0.0.5 -f ./PMFightAcademy.Client/Dockerfile ./
docker build -t registry.digitalocean.com/pm-fight-backend/pmfightacademyadmin:0.0.0.5 -f ./PMFightAcademy.Admin/Dockerfile ./
docker login registry.digitalocean.com/pm-fight-backend
docker push registry.digitalocean.com/pm-fight-backend/pmfightacademyclient:0.0.0.5
docker push registry.digitalocean.com/pm-fight-backend/pmfightacademyadmin:0.0.0.5
