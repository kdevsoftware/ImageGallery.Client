import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

import 'rxjs/add/operator/first';
import 'rxjs/add/operator/toPromise';

import { GalleryService } from '../../../gallery.service';
import { IGalleryIndexViewModel, IImage } from '../../../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { take } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

@Component({
  selector: 'app-album-view',
  templateUrl: './album-view.component.html',
  styleUrls: ['./album-view.component.scss'],
  providers: [GalleryService]
})
export class AlbumViewComponent implements OnInit {
  albumId;
  albumViewModel: IGalleryIndexViewModel;

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

  constructor(
    private readonly galleryService: GalleryService,
    private activatedRoute: ActivatedRoute,
    public toastr: ToastrService,
    private spinnerService: NgxLoadingSpinnerService,
    private changeDetectorRef: ChangeDetectorRef,
    private modalService: BsModalService) {
  }

  async ngOnInit() {
    this.title = localStorage.getItem('album-title');
    this.pagination.limit = localStorage.getItem('album-view-limit') ? parseInt(localStorage.getItem('album-view-limit')) : 15;
    this.pagination.page = localStorage.getItem('album-view-page') ? parseInt(localStorage.getItem('album-view-page')) : 1;
    this.activatedRoute.paramMap
      .pipe(take(1))
      .subscribe((paramMap: any) => {
        this.albumId = paramMap.params['id'];
        this.getAlbumViewModel();
      });
  }

  public openDeleteModal(image, template) {
    this.imageToDelete = image;
    this.modalRef = this.modalService.show(template);
  }

  public daleteImage() {
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

  private getAlbumViewModel(event?) {
    this.spinnerService.show();

    if (typeof event == 'string') {
      this.pagination.limit = parseInt(event);
      this.pagination.page = 1;
      localStorage.setItem('album-view-limit', this.pagination.limit.toString());
      localStorage.setItem('album-view-page', this.pagination.page.toString());
    } else if (typeof event == 'object') {
      this.pagination.limit = event.itemsPerPage;
      this.pagination.page = event.page;
      localStorage.setItem('album-view-limit', this.pagination.limit.toString());
      localStorage.setItem('album-view-page', this.pagination.page.toString());
    }
    setTimeout(() => {
      this.pagination.page = localStorage.getItem('album-view-page') ? parseInt(localStorage.getItem('album-view-page')) : 1;
      this.changeDetectorRef.detectChanges();
    }, 1000);

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
