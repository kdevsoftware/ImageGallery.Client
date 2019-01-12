import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { GalleryService } from '../../../gallery.service';

@Component({
  selector: 'app-image-map',
  templateUrl: './image-map.component.html',
  styleUrls: ['./image-map.component.scss'],
  providers: [GalleryService]
})
export class ImageMapComponent implements OnInit {
  @ViewChild('gmap') gmapElement: any;
  @Input() image: any;

  map: google.maps.Map;

  constructor() {}

  ngOnInit() {
    this.image.map.center.lat = this.image.map.center.latitude;
    this.image.map.center.lng = this.image.map.center.longitude;

    const markerLocation = new google.maps.LatLng(this.image.loc.lat, this.image.loc.lon);

    const map = new google.maps.Map(
      this.gmapElement.nativeElement,
      this.image.map
    );

    var marker = new google.maps.Marker({
      position: markerLocation,
      title: this.image.title
    });

    marker.setMap(map);
  }
}
