var currentFeature
var currentMarker

class FeatureType {
    static empty = "Empty"
    static unchecked = "Unchecked"
    static checked = "Checked"
}

class ServerFeatureRepository {
    async getAll() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow',
        };

        let response = await fetch("https://localhost:44395/api/markers/all", requestOptions);
        let markers = await response.json();

        let features = this.markersToFeatures(markers)

        return features
    }

    markersToFeatures(markers) {
        let features = []
        for (const marker of markers) {
            let feature = this.markerToFeature(marker)
            features.push(feature)
        }

        return features
    }

    markerToFeature(marker) {
        var markerType = this.intToFeatureType(marker.markerType)

        return {
            markerId: marker.id,
            coordinates: [marker.x, marker.y],
            type: markerType,
            audioStatus: marker.audioStatus,
            title: marker.title
        }
    }

    intToFeatureType(valueFeatureType) {
        if (valueFeatureType === 0) {
            return FeatureType.empty
        } else if (valueFeatureType === 1) {
            return FeatureType.unchecked
        }
        else if (valueFeatureType === 2) {
            return FeatureType.checked
        }
    }

    featureTypeToMarkerType(featureType) {
        if (featureType === FeatureType.empty) {
            return 0
        } else if (featureType === FeatureType.unchecked) {
            return 1
        } else if (featureType === FeatureType.checked) {
            return 2
        }
    }

    featureToMarker(feature) {
        var markerTypeValue = this.featureTypeToMarkerType(feature.type)
        return {
            id: feature.markerId,
            x: feature.coordinates[0].toString(),
            y: feature.coordinates[1].toString(),
            markerType: markerTypeValue,
            title: feature.title,
            audioStatus: feature.audioStatus
        }
    }

    async setFeature(feature) {
        var marker = this.featureToMarker(feature)
        var headers = new Headers();
        headers.append("Content-Type", "application/json");

        var raw = JSON.stringify(marker);

        var requestOptions = {
            method: 'POST',
            headers: headers,
            body: raw,
            redirect: 'follow'
        };

        let response = await fetch("https://localhost:44395/api/markers/add", requestOptions)
        let answerMarker = await response.json();
        var answerFeature = this.markerToFeature(answerMarker)
        return answerFeature
    }

    updateFeature(feature) {
        var marker = this.featureToMarker(feature)

        var myHeaders = new Headers();
        myHeaders.append("Content-Type", "application/json");

        var raw = JSON.stringify(marker);

        var requestOptions = {
            method: 'PUT',
            headers: myHeaders,
            body: raw,
            redirect: 'follow'
        };

        fetch("https://localhost:44395/api/markers/edit", requestOptions)
    }

    delete(feature) {
        var requestOptions = {
            method: 'DELETE',
            redirect: 'follow'
        };

        fetch("https://localhost:44395/api/markers/delete/" + feature.markerId, requestOptions)
    }
}

class MapFeatureRepository {
    constructor(map, mapboxManager) {
        this.map = map
        this.mapboxManager = mapboxManager
    }

    setFeature(feature) {
        // create a HTML element for each feature
        const markerElement = document.createElement('div');

        if (feature.type === FeatureType.empty) {
            markerElement.className = 'empty-point';
        } else if (feature.type === FeatureType.unchecked) {
            markerElement.className = 'unchecked-point'
        } else if (feature.type === FeatureType.checked) {
            markerElement.className = 'checked-point'
        }

        // make a marker for each feature and add to the map
        var marker = new mapboxgl
            .Marker(markerElement);

        markerElement.addEventListener('click', function () {
            currentFeature = feature
            currentMarker = marker
        });

        const listenButtonHTML = '<h3><button type="button" onclick="listenCurrentFeature()">Прослушать</button></h3>'
        const acceptButtonHTML = '<h3><button type="button" onclick="acceptCurrentFeature()">Принять</button></h3>'
        const rejectButtonHTML = '<h3><button type="button" onclick="rejectCurrentFeature()">Отклонить</button></h3>'
        const deleteButtonHTML = '<h3><button type="button" onclick="deleteCurrentFeature()">Удалить</button></h3>'

        var popupHTML = ''
        if (feature.type === FeatureType.unchecked) {
            popupHTML = popupHTML + listenButtonHTML + acceptButtonHTML + rejectButtonHTML
        } else if (feature.type === FeatureType.checked) {
            popupHTML = popupHTML + listenButtonHTML + rejectButtonHTML
        }
        popupHTML = popupHTML + deleteButtonHTML

        marker
            .setLngLat(feature.coordinates)
            .setPopup(
                new mapboxgl.Popup({ offset: 25 }) // add popups
                    .setHTML(popupHTML)
            )
            .addTo(this.map);
    }

    deleteCurrentMarker() {
        currentMarker.remove()
    }
}

class FeatureRepository {
    constructor(serverFeatureRepository, mapFeatureRepository) {
        this.serverFeatureRepository = serverFeatureRepository
        this.mapFeatureRepository = mapFeatureRepository
    }

    setFeature(feature) {
        this.serverFeatureRepository
            .setFeature(feature)
            .then((createdFeature) => {
                this.mapFeatureRepository.setFeature(createdFeature)
            })
    }

    updateFeature(feature) {
        this.serverFeatureRepository.updateFeature(feature)
        this.mapFeatureRepository.deleteCurrentMarker()
        this.mapFeatureRepository.setFeature(feature)
    }

    deleteFeature(feature) {
        this.serverFeatureRepository.delete(feature)
        this.mapFeatureRepository.deleteCurrentMarker()
    }
}


class MapboxManager {
    constructor(serverFeatureRepository) {
        this.serverFeatureRepository = serverFeatureRepository
    }

    setStoredPoints() {
        // add markers to map
        this.serverFeatureRepository.getAll().then((features) => {
            for (const feature of features) {
                this.mapFeatureRepository.setFeature(feature)
            }
        })
    }

    run() {
        this.initialize()
        this.setStoredPoints()
    }

    initialize() {
        mapboxgl.accessToken = 'pk.eyJ1IjoidmxhZGltaXJndWxpYWV2IiwiYSI6ImNrdjB6N2c2ZjM1MDUyb2xuMnRjMHh6M3cifQ.8Z9Z5v4XxfdgaNhlDhFh1w';

        this.map = new mapboxgl.Map({
            container: 'map',
            style: 'mapbox://styles/mapbox/light-v10',
            center: [-96, 37.8],
            zoom: 3
        });

        this.mapFeatureRepository = new MapFeatureRepository(this.map, this)
        this.featureRepository = new FeatureRepository(this.serverFeatureRepository, this.mapFeatureRepository)
    }

    setEmptyFeature() {
        const oldCursor = this.map.getCanvas().style.cursor

        this.map.getCanvas().style.cursor = 'pointer';

        this.map.once('click', (e) => {
            const feature = {
                coordinates: [e.lngLat.lng, e.lngLat.lat],
                type: FeatureType.empty
            };

            this.featureRepository.setFeature(feature)
            this.map.getCanvas().style.cursor = oldCursor;
        });
    }

    listenCurrentFeature() {
        var audio = new Audio(currentFeature.audioLink);
        audio.play();
    }

    acceptCurrentFeature() {
        currentFeature.type = FeatureType.checked

        this.featureRepository.updateFeature(currentFeature)
    }

    rejectCurrentFeature() {
        currentFeature.type = FeatureType.empty

        this.featureRepository.updateFeature(currentFeature)
    }

    deleteCurrentFeature() {
        this.featureRepository.deleteFeature(currentFeature)
    }
}


const serverFeatureRepository = new ServerFeatureRepository()
const mapboxManager = new MapboxManager(serverFeatureRepository)

mapboxManager.run()

function setEmptyFeature() {
    mapboxManager.setEmptyFeature()
}

function listenCurrentFeature() {
    mapboxManager.listenCurrentFeature()
}

function acceptCurrentFeature() {
    mapboxManager.acceptCurrentFeature()
}

function rejectCurrentFeature() {
    mapboxManager.rejectCurrentFeature()
}

function deleteCurrentFeature() {
    mapboxManager.deleteCurrentFeature()
}
