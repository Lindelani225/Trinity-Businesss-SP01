var map;
var marker;
if (navigator.geolocation) {
    // watch for user movement
    navigator.geolocation.watchPosition(function (position) {
        var lat = position.coords.latitude;
        var lng = position.coords.longitude;
        console.log(lat, lng);
        var myLatLng = { lat: lat, lng: lng }
        initMap(myLatLng);
    });
} else {
    alert("Geolocation is not supported by this browser.");
}
function initMap(myLatLng) {
    // create the map if it doesn't exist yet
    if (!map) {
        map = new google.maps.Map(document.getElementById('map'), {
            center: myLatLng,
            zoom: 17,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });
    }
    // optional for centering the map on each user movement:
    else {
        map.setCenter(myLatLng)
    }
    // create the marker if it doesn't exist yet
    if (!marker) {
        marker = new google.maps.Marker({
            position: myLatLng,
            map: map,
            title: 'My Location'
        });
        // Marker Animation
                marker.setAnimation(google.maps.Animation.BOUNCE);
                google.maps.Marker
                marker.addListener("click", toggleBounce);

                function toggleBounce() {
                    if (marker.getAnimation() !== null) {
                        marker.setAnimation(null);
                   } else {
                      marker.setAnimation(google.maps.Animation.BOUNCE);
                  }
            }
    } else {
        // update the markers position
        marker.setPosition(myLatLng);
    }
}
