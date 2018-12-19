import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

import 'rxjs/add/operator/first';
import 'rxjs/add/operator/toPromise';

import { GalleryService } from '../../../gallery.service';
import { IGalleryIndexViewModel, IImage, IAlbum } from '../../../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { take } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { StorageService } from '../../../services/storage.service';
import { TitleService } from '../../../services/title.service';

@Component({
  selector: 'app-album-view',
  templateUrl: './album-view.component.html',
  styleUrls: ['./album-view.component.scss'],
  providers: [GalleryService]
})
export class AlbumViewComponent implements OnInit {
  albumId;
  albumViewModel: IGalleryIndexViewModel;
  albumDetails: IAlbum;
  albumImages: IGalleryIndexViewModel;

  pagination = {
    page: 1,
    limit: 15,
    totalItems: 10
  };
  perPage = [15, 30, 60, 90];
  title = '';

  categories: string[] = ['Landscapes', 'Portraits', 'Animals'];

  modalRef: BsModalRef;
  private imageToDelete;
  private clicked: boolean = false;

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
        this.getAlbumDetails(this.albumId);
        this.getAlbumImages(this.albumId);
      });
  }

  public onSubmitTitle(image: IImage, input: any, event?: any) {
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

  public onCancelEditTitle(image: IImage, input: any) {
    if (this.clicked) {
      image.title = input.value;
    } else {
      input.value = image.title;
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

  private getAlbumImages(id: string) {
    this.galleryService.getAlbumViewModel(this.albumId, null, null)
      .then((response: any) => {
        this.albumImages = response.images;
      }).catch(() => {
        this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
      });
  }

  private getAlbumDetails(id: string) {
    this.galleryService.getAlbum(id)
      .subscribe(
        (response: IAlbum) => {
          this.albumDetails = response;
        },
        (error) => {
          this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
          console.log(error);
        }
      );
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
