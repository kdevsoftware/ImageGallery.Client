import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

import 'rxjs/add/operator/first';
import 'rxjs/add/operator/toPromise';

import { GalleryService } from '../../../gallery.service';
import { IGalleryIndexViewModel, IImage, IAlbumMetadata } from '../../../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { take } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { StorageService } from '../../../services/storage.service';
import { TitleService } from '../../../services/title.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-album-view',
  templateUrl: './album-view.component.html',
  styleUrls: ['./album-view.component.scss'],
  providers: [GalleryService]
})
export class AlbumViewComponent implements OnInit {
  albumId;
  albumViewModel: IGalleryIndexViewModel;
  albumDetails: IAlbumMetadata;
  albumImages: IGalleryIndexViewModel;

  pagination = {
    page: 1,
    limit: 15,
    totalItems: 10
  };
  perPage = [15, 30, 60, 90];
  title = '';

  categories: string[] = ['Landscapes', 'Portraits', 'Animals'];

  inputTags: any[] = [];
  modalRef: BsModalRef;
  private imageToDelete;
  private clicked: boolean = false;
  private tagIndex;

  constructor(
    private readonly galleryService: GalleryService,
    private activatedRoute: ActivatedRoute,
    public toastr: ToastrService,
    private spinnerService: NgxLoadingSpinnerService,
    private modalService: BsModalService,
    public storage: StorageService,
    private titleService: TitleService) {
  }

  async ngOnInit() {
    this.titleService.set('Album View');
    this.title = this.storage.get('album-title');
    this.pagination.limit = this.storage.get('album-view-limit') ? parseInt(this.storage.get('album-view-limit')) : 15;
    this.pagination.page = 1;
    this.activatedRoute.paramMap
      .pipe(take(1))
      .subscribe((paramMap: any) => {
        this.albumId = paramMap.params['id'];
        this.getAlbumViewModel();
      });
  }

  public onAlbumTabClick() {
    this.getAlbumViewModel();
  }

  public onMetadataTabClick() {
    this.getAlbumMetadata(this.albumId);
  }

  public onDeselectMetadata() {
    this.inputTags = [];
  }

  public onAddTag(tag: any) {
    this.galleryService.postAlbumMetadataTag(this.albumId, tag.display)
      .subscribe(
        () => {
          // todo get id from server
          this.toastr.success('Tag has been added successfully!', 'Success!', { closeButton: true });
        },
        (err: any) => {
          this.inputTags.pop();
          this.toastr.error('Failed to add tag!', 'Oops!', { closeButton: true });
        });
  }

  public onRemovingTag = (tag: any): Observable<any> => {
    this.tagIndex = this.inputTags.indexOf(tag);
    return Observable.of(tag);
  }

  public onRemoveTag(tag: any) {
    this.galleryService.deleteAlbumMetadataTag(this.albumId, tag.value)
      .subscribe(
        () => {
          this.toastr.success('Tag has been removed successfully!', 'Success!', { closeButton: true });
        },
        () => {
          this.inputTags.splice(this.tagIndex, 0, tag);
          this.toastr.error('Failed to remove tag!', 'Oops!', { closeButton: true });
        }
      );
  }

  public onSortAlbumTabClick() {
    this.getAlbumImages(this.albumId);
  }

  public onSubmitImageTitle(image: IImage, input: any, event?: any) {
    this.clicked = true;
    const tempTitle = image.title;

    if (input.value !== image.title) {
      this.galleryService.patchImageTitle(image.id, 'title', input.value)
        .subscribe(
          () => {
            this.toastr.success('Title has been updated successfully!', 'Success!', { closeButton: true });
          },
          (err) => {
            if (err.status === 500) {
              this.toastr.error('Application Error has occurred', 'Oops!', { closeButton: true });
            } else {
              this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            }
            input.value = tempTitle;
          }
        );
    }
    if (event) {
      event.target.blur();
    }
  }

  public onCancelEditImageTitle(image: IImage, input: any) {
    if (this.clicked) {
      image.title = input.value;
    } else {
      input.value = image.title;
    }
    this.clicked = false;
  }

  public onSubmitAlbumTitle(album: IAlbumMetadata, inputTitle: any, event?: any) {
    this.clicked = true;
    const tempTitle = album.title;

    if (inputTitle.value !== album.title) {
      this.galleryService.patchAlbumProperty(album.id, 'title', inputTitle.value)
        .subscribe(
          () => {
            this.title = inputTitle.value;
            this.toastr.success('Album title has been updated successfully!', 'Success!', { closeButton: true });
          },
          (err) => {
            if (err.status === 500) {
              this.toastr.error('Application Error has occurred', 'Oops!', { closeButton: true });
            } else {
              this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            }
            inputTitle.value = tempTitle;
          }
        );
    }
    if (event) {
      event.target.blur();
    }
  }

  public onCancelEditAlbumTitle(album: IAlbumMetadata, inputTitle: any) {
    if (this.clicked) {
      album.title = inputTitle.value;
    } else {
      inputTitle.value = album.title;
    }
    this.clicked = false;
  }

  public onSubmitAlbumDescription(album: IAlbumMetadata, inputDescr: any, event?: any) {
    this.clicked = true;
    const tempDescr = album.description;

    if (inputDescr.value !== album.description) {
      this.galleryService.patchAlbumProperty(album.id, 'description', inputDescr.value)
        .subscribe(
          () => {
            this.toastr.success('Album description has been updated successfully!', 'Success!', { closeButton: true });
          },
          (err) => {
            if (err.status === 500) {
              this.toastr.error('Application Error has occurred', 'Oops!', { closeButton: true });
            } else {
              this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            }
            inputDescr.value = tempDescr;
          }
        );
    }
    if (event) {
      event.target.blur();
    }
  }

  public onCancelEditAlbumDescription(album: IAlbumMetadata, inputDescr: any) {
    if (this.clicked) {
      album.description = inputDescr.value;
    } else {
      inputDescr.value = album.description;
    }
    this.clicked = false;
  }

  public openDeleteModal(image, template) {
    this.imageToDelete = image;
    this.modalRef = this.modalService.show(template);
  }

  public deleteImage() {
    this.spinnerService.show();

    this.galleryService.deleteImageFromAlbum(this.albumId, this.imageToDelete.id)
      .subscribe(
        () => {
          this.toastr.success('Image has been deleted successfully!', 'Success!', { closeButton: true });
          this.getAlbumViewModel();
          this.modalRef.hide();
          this.spinnerService.hide();
        },
        () => {
          this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
          this.modalRef.hide();
          this.spinnerService.hide();
        }
      );
  }

  public onImagesSorted(albumImagesSorted: IGalleryIndexViewModel) {
    this.galleryService.updateAlbumImages(this.albumId, albumImagesSorted);
  }

  public updatePrimaryImage(image: IImage) {
    if (!image.isPrimaryImage) {
      this.spinnerService.show();

      this.galleryService.setPrimaryAlbumImage(this.albumId, image.id)
        .subscribe(
          () => {
            this.toastr.success('Primary Image has been updated successfully!', 'Success!', { closeButton: true });
            this.spinnerService.hide();
          },
          () => {
            this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            this.spinnerService.hide();
          }
        );

      this.albumViewModel.images
        .forEach((imageItem: IImage) => {
          if (imageItem.isPrimaryImage) {
            imageItem.isPrimaryImage = false;
          }
        });
    }

    image.isPrimaryImage = true;
  }

  private getAlbumMetadata(id: string) {
    this.spinnerService.show();

    this.galleryService.getAlbumMetadata(id)
      .subscribe(
        (response: IAlbumMetadata) => {
          this.albumDetails = response;
          this.inputTags = this.albumDetails.albumTags
            .map((tag) => {
              return {
                display: tag.tag,
                value: tag.id
              };
            });
          this.spinnerService.hide();
        },
        () => {
          this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
          this.spinnerService.hide();
        }
      );
  }

  private getAlbumImages(id: string) {
    this.spinnerService.show();

    this.galleryService.getAlbumViewModel(id, null, null)
      .then((response: any) => {
        this.albumImages = response.images;
        this.spinnerService.hide();
      }).catch(() => {
        this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
        this.spinnerService.hide();
      });
  }

  private getAlbumViewModel(event?) {
    this.spinnerService.show();

    if (typeof event == 'string') {
      this.pagination.limit = parseInt(event);
      this.pagination.page = 1;
      this.storage.set('album-view-limit', this.pagination.limit.toString());
    } else if (typeof event == 'object') {
      this.pagination.limit = event.itemsPerPage;
      this.pagination.page = event.page;
      this.storage.set('album-view-limit', this.pagination.limit.toString());
    }

    this.galleryService.getAlbumViewModel(this.albumId, this.pagination.limit, this.pagination.page)
      .then((response: any) => {
        this.albumViewModel = response.images;
        this.pagination.totalItems = response.totalCount;
        this.spinnerService.hide();
      }).catch(() => {
        this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
        this.spinnerService.hide();
      });
  }
}
