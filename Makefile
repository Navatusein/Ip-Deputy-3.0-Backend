container-name = ip-deputy-backend
image-name = navatusein/$(container-name)

build:
	docker images -f=reference="$(image-name):*" --format "{{.ID}}" | xargs docker rmi -f
	docker build --build-arg version=$(v) -t $(image-name):$(v) -t $(image-name):latest .
push:
	docker push $(image-name) -a
run:
	docker run --rm --name $(container-name) $(image-name)
publish:
	make build v=$(v)
	make push
dev:
	make build v=$(v)
	make run v=$(v)
	