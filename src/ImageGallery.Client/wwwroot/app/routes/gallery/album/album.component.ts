import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryService } from '../../../gallery.service';
import { IAlbumIndexViewModel, IRouteTypeModel, IAlbum } from '../../../shared/interfaces';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { StorageService } from '../../../services/storage.service';


@Component({
  selector: 'app-album',
  templateUrl: './album.component.html',
  styleUrls: ['./album.component.scss'],
  providers: [GalleryService]
})
export class AlbumComponent implements OnInit {

  albumIndexViewModel: IAlbumIndexViewModel;
  typeModel: IRouteTypeModel;

  type: string;

  pagination: any = {};
  perPage = [15, 30, 60, 90];

  modalRef: BsModalRef;
  private albumToDelete;
  private editedTitle: boolean = false;
  private editedDescription: boolean = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private authService: AuthService,
    private galleryService: GalleryService,
    public toastr: ToastrService,
    public storage: StorageService,
    private spinnerService: NgxLoadingSpinnerService,
    private router: Router,
    private modalService: BsModalService) {}

  ngOnInit() {
    this.type = this.activatedRoute.snapshot.params.type;

    this.authService.getIsAuthorized().subscribe(
      (isAuthorized: boolean) => {
        if (isAuthorized) {
          const limit = this.storage.getPerUser('album-limit');
          const page = this.storage.getPerUser('album-page');
          this.pagination.limit = limit ? parseInt(limit) : 15;
          this.pagination.page = page ? parseInt(page) : 1;
          this.loadAlbumModdel(this.pagination.limit, this.pagination.page);
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

  onClickProperty(event: any) {
    event.stopPropagation();
  }

  onEnterTitle(album: IAlbum, inputTitle: any, event: any) {
    if (inputTitle.value !== album.title) {
      this.spinnerService.show();

      this.galleryService.patchAlbumTitle(album.id, 'title', inputTitle.value)
        .subscribe(
          () => {
            this.toastr.success('Title has been updated successfully!', 'Success!', { closeButton: true });
            this.editedTitle = true;
            event.target.blur();
            this.spinnerService.hide();
          },
          () => {
            this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            this.editedTitle = false;
            event.target.blur();
            this.spinnerService.hide();
          }
        );
    } else {
      this.editedTitle = false;
    }
  }

  onCancelEditTitle(album: IAlbum, inputTitle: any) {
    if (this.editedTitle) {
      album.title = inputTitle.value;
      this.editedTitle = false;
    } else {
      inputTitle.value = album.title;
    }
  }

  onEnterDescr(album: IAlbum, inputDescr: any, event: any) {
    if (inputDescr.value !== album.description) {
      this.spinnerService.show();

      this.galleryService.patchAlbumDescription(album.id, 'description', inputDescr.value)
        .subscribe(
          () => {
            this.toastr.success('Description has been updated successfully!', 'Success!', { closeButton: true });
            this.editedDescription = true;
            event.target.blur();
            this.spinnerService.hide();
          },
          () => {
            this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
            this.editedDescription = false;
            event.target.blur();
            this.spinnerService.hide();
          }
        );
    } else {
      this.editedDescription = false;
    }
  }

  onCancelEditDescr(album: IAlbum, inputDescr: any) {
    if (this.editedDescription) {
      album.description = inputDescr.value;
      this.editedDescription = false;
    } else {
      inputDescr.value = album.description;
    }
  }

  public openDeleteModal(album, template, event) {
    this.albumToDelete = album;
    this.modalRef = this.modalService.show(template);

    event.stopPropagation();
  }

  public daleteAlbum() {
    this.spinnerService.show();

    this.galleryService.deleteAlbumViewModel(this.albumToDelete.id)
      .subscribe(
        () => {
          this.toastr.success('Album has been deleted successfully!', 'Success!', { closeButton: true });
          this.loadAlbumModdel(this.pagination.limit, this.pagination.page);
        },
        () => this.toastr.error('Access is denied!', 'Oops!', { closeButton: true }),
        () => {
          this.modalRef.hide();
          this.spinnerService.hide();
        }
      );
  }

  public getAlbumIndexViewModel(event) {
    const page = event.page;

    if (page !== this.pagination.page) {
      this.storage.setPerUser('album-page', page.toString());
      this.loadAlbumModdel(this.pagination.limit, page);
    }
  }

  public updateLimit(event) {
    const limit = parseInt(event);

    this.pagination.limit = limit;

    this.storage.setPerUser('album-limit', limit.toString());

    if (this.pagination.page === 1) {
      this.loadAlbumModdel(limit, this.pagination.page);
    } else {
      setTimeout(() => {
        this.pagination.page = 1;
        this.storage.setPerUser('album-page', this.pagination.page.toString());
        this.loadAlbumModdel(limit, this.pagination.page);
      });
    }
  }

  private scrollToTop() {
    window.scrollTo(0, 0);
  }

  goToAlbumView(album) {
    this.storage.set('album-title', album.title);
    this.router.navigate(['album-view', album.id]);
  }

  private loadAlbumModdel(limit, page) {
    this.spinnerService.show();

    this.galleryService.getAlbumIndexViewModel(limit, page)
      .then((response: any) => {
        this.albumIndexViewModel = response.images;
        this.pagination.totalItems = response.totalCount;
        console.log(response);
        this.scrollToTop();
        this.spinnerService.hide();
      }).catch(() => {
        this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
        this.spinnerService.hide();
      });
  }
}
