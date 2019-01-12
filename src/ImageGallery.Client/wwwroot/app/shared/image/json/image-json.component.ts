import { Component, OnInit, Input } from '@angular/core';
import { GalleryService } from '../../../gallery.service';

@Component({
  selector: 'app-image-json',
  templateUrl: './image-json.component.html',
  styleUrls: ['./image-json.component.scss'],
  providers: [GalleryService]
})
export class ImageJsonComponent implements OnInit {
  @Input() image: any;
  text: string = '';
  content = '<strong>Hi</strong>';
  options: any = { printMargin: true };

  constructor() {}

  ngOnInit() {
    this.text = JSON.stringify(this.image, null, 2);
  }
}
