import { Component, OnInit, Input } from '@angular/core';
import { GalleryService } from '../../../gallery.service';

@Component({
  selector: 'app-image-map',
  templateUrl: './image-map.component.html',
  styleUrls: ['./image-map.component.scss'],
  providers: [GalleryService]
})
export class ImageMapComponent implements OnInit {
  @Input() image: any;

  constructor() {}

  ngOnInit() {
  }
}
