import { Component, OnInit, Input } from "@angular/core";
import { IGalleryIndexViewModel } from "../../../../shared/interfaces";


@Component({
  selector: 'app-album-sort',
  templateUrl: './album-sort.component.html',
  styleUrls: ['./album-sort.component.scss'],
})
export class AlbumSortComponent implements OnInit {
  @Input() albumImages: IGalleryIndexViewModel;

  ngOnInit() {

  }
}