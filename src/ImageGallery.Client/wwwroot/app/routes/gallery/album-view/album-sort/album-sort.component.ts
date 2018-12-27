import { Component, OnInit, Input, ViewChild, AfterViewInit, Output, EventEmitter } from "@angular/core";
import { CdkDrag, CdkDropList, CdkDropListContainer, CdkDropListGroup, moveItemInArray } from "@angular/cdk/drag-drop";

import { IGalleryIndexViewModel } from "../../../../shared/interfaces";


@Component({
  selector: 'app-album-sort',
  templateUrl: './album-sort.component.html',
  styleUrls: ['./album-sort.component.scss']
})
export class AlbumSortComponent implements OnInit, AfterViewInit {
  @ViewChild(CdkDropListGroup) listGroup: CdkDropListGroup<CdkDropList>;
  @ViewChild(CdkDropList) placeholder: CdkDropList;
  @Input() albumImages: IGalleryIndexViewModel;
  @Output() albumImagesSorted = new EventEmitter<IGalleryIndexViewModel>();

  public target: CdkDropList;
  public targetIndex: number;
  public source: CdkDropListContainer;
  public sourceIndex: number;

  constructor() {
    this.target = null;
    this.source = null;
  }

  ngOnInit() {
    this.albumImages = { images: [] } as IGalleryIndexViewModel;
  }

  ngAfterViewInit() {
    let phElement = this.placeholder.element.nativeElement;
    phElement.style.display = 'none';
    phElement.parentNode.removeChild(phElement);
  }

  public onDropImage() {
    if (!this.target)
      return;

    let phElement = this.placeholder.element.nativeElement;
    let parent = phElement.parentNode;

    phElement.style.display = 'none';

    parent.removeChild(phElement);
    parent.appendChild(phElement);
    parent.insertBefore(this.source.element.nativeElement, parent.children[this.sourceIndex]);

    this.target = null;
    this.source = null;

    if (this.sourceIndex != this.targetIndex) {
      moveItemInArray(this.albumImages.images, this.sourceIndex, this.targetIndex);
    }

    this.albumImages.images.forEach((image) => {
      image.sort = this.albumImages.images.indexOf(image) + 1;
    });

    this.albumImagesSorted.emit(this.albumImages);
  }

  enteredImage = (drag: CdkDrag, drop: CdkDropList) => {
    if (drop == this.placeholder)
      return true;

    let phElement = this.placeholder.element.nativeElement;
    let dropElement = drop.element.nativeElement;

    let dragIndex = __indexOf(dropElement.parentNode.children, drag.dropContainer.element.nativeElement);
    let dropIndex = __indexOf(dropElement.parentNode.children, dropElement);

    if (!this.source) {
      this.sourceIndex = dragIndex;
      this.source = drag.dropContainer;

      let sourceElement = this.source.element.nativeElement;
      phElement.style.width = sourceElement.clientWidth + 'px';
      phElement.style.height = sourceElement.clientHeight + 'px';

      sourceElement.parentNode.removeChild(sourceElement);
    }

    this.targetIndex = dropIndex;
    this.target = drop;

    phElement.style.display = '';
    dropElement.parentNode.insertBefore(phElement, (dragIndex < dropIndex)
      ? dropElement.nextSibling : dropElement);

    this.source.start();
    this.placeholder.enter(drag, drag.element.nativeElement.offsetLeft, drag.element.nativeElement.offsetTop);

    return false;
  }
}

function __indexOf(collection, node) {
  return Array.prototype.indexOf.call(collection, node);
};