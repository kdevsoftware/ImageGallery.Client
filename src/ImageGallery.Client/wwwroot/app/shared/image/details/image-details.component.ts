import { Component, OnInit, Input } from '@angular/core';
import { GalleryService } from '../../../gallery.service';

@Component({
  selector: 'app-image-details',
  templateUrl: './image-details.component.html',
  styleUrls: ['./image-details.component.scss'],
  providers: [GalleryService]
})
export class ImageDetailsComponent implements OnInit {
  @Input() image: any;

  constructor() {}

  ngOnInit() {}
}
