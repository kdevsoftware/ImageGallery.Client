import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryService } from '../../../gallery.service';
import { IGalleryIndexViewModel, IRouteTypeModel } from '../../../shared/interfaces';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { StorageService } from '../../../services/storage.service';


@Component({
  selector: 'app-gallery',
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss'],
  providers: [GalleryService]
})
export class GalleryComponent implements OnInit {

  galleryIndexViewModel: IGalleryIndexViewModel;
  typeModel: IRouteTypeModel;

  type: string;

  pagination: any = {};
  page = 0;
  limit: number;
  perPage = [15, 30, 60, 90];
  albums = [];
  savedAlbums = [];

  modalRef: BsModalRef;
  flickrList = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private authService: AuthService,
    private galleryService: GalleryService,
    public toastr: ToastrService,
    public storage: StorageService,
    private spinnerService: NgxLoadingSpinnerService,
    private modalService: BsModalService) {
    this.savedAlbums = JSON.parse(this.storage.get('albums')) ? JSON.parse(this.storage.get('albums')) : [];
  }

  ngOnInit() {
    this.type = this.activatedRoute.snapshot.params.type;
    this.authService.getIsAuthorized().subscribe(
      (isAuthorized: boolean) => {
        if (isAuthorized) {
          this.limit = this.storage.getPerUser('limit') ? parseInt(this.storage.getPerUser('limit')) : 15;
          this.page = this.storage.getPerUser('page') ? parseInt(this.storage.getPerUser('page')) : 1;
          this.loadGalleryModel(this.limit, this.page);
        }
      }
    );
  }

  ngAfterViewInit() {
    if (this.storage.get('isEdited') == 'yes') {
      this.toastr.toastrConfig.closeButton = true;
      this.toastr.success('Image has been edited successfully!', 'Success!', { closeButton: true });
      this.storage.remove('isEdited');
    } else if (this.storage.get('isAdded') == 'yes') {
      this.toastr.success('Image has been added successfully!', 'Success!', { closeButton: true });
      this.storage.remove('isAdded');
    }
  }

  public deleteImage(imageId: string) {
    this.galleryService.deleteImageViewModel(imageId)
      .subscribe((response) => { },
        (err: any) => {
          this.toastr.error('Failed to delete image!', 'Oops!', { closeButton: true });
          console.log(err);
        },
        () => {
          this.toastr.success('Image has been deleted successfully!', 'Success!', { closeButton: true });
          this.galleryIndexViewModel.images = this.galleryIndexViewModel.images.filter(x => x.id != imageId);
        });
  }

  public getGalleryIndexViewModel(event) {
    const page = event.page;

    if (page !== this.page) {
      this.storage.setPerUser('page', page.toString());
      this.loadGalleryModel(this.limit, page);
    }
  }

  public updateLimit(event) {
    const limit = parseInt(event);

    this.limit = limit;

    this.storage.setPerUser('limit', limit.toString());

    if (this.page === 1) {
      this.loadGalleryModel(limit, this.page);
    } else {
      setTimeout(() => {
        this.page = 1;
        this.storage.setPerUser('page', this.page.toString());
        this.loadGalleryModel(limit, this.page);
      });
    }
  }

  private scrollToTop() {
    window.scrollTo(0, 0);
  }

  public saveAlbum(index) {
    this.albums[index] = !this.albums[index];
    if (this.albums[index]) {
      this.savedAlbums.push(this.galleryIndexViewModel.images[index]);
    }
    this.storage.set('albums', JSON.stringify(this.savedAlbums));
  }

  public showModal(photo, template) {
    this.spinnerService.show();
    this.galleryService.getPhotoAttraction(photo.photoId).subscribe((res: any) => {
      this.flickrList = res;
      this.modalRef = this.modalService.show(template);
      this.spinnerService.hide();
    });
  }

  private loadGalleryModel(limit, page) {
    this.spinnerService.show();

    this.galleryService.getGalleryIndexViewModel(limit, page)
      .then((response: any) => {
        this.galleryIndexViewModel = response.images;
        this.galleryIndexViewModel.images.forEach((image, i) => {
          this.albums[i] = this.savedAlbums.findIndex(album => album.id === image.id) > -1;
        });
        this.pagination.totalItems = response.totalCount;
        this.scrollToTop();

        this.spinnerService.hide();
      }).catch(() => {
        this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
        this.spinnerService.hide();
      });
  }
}
