import { Component, OnInit, ViewContainerRef, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryService } from '../../../gallery.service';
import { IAlbumIndexViewModel, IRouteTypeModel, IAlbum } from '../../../shared/interfaces';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { setTimeout } from 'timers';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';


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

    pagination = {
        page: 1,
        limit: 15,
        totalItems: 10
    };
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
        vcr: ViewContainerRef,
        private spinnerService: NgxLoadingSpinnerService,
        private changeDetectorRef: ChangeDetectorRef,
        private router: Router,
        private modalService: BsModalService
    ) {
        //this.toastr.setRootViewContainerRef(vcr);
    }

    ngOnInit() {
        this.type = this.activatedRoute.snapshot.params.type;

        this.authService.getIsAuthorized().subscribe(
            (isAuthorized: boolean) => {
                if (isAuthorized) {
                    this.pagination.limit = localStorage.getItem('album-limit') ? parseInt(localStorage.getItem('album-limit')) : 15;
                    this.pagination.page = localStorage.getItem('album-page') ? parseInt(localStorage.getItem('album-page')) : 1;
                    this.getAlbumIndexViewModel();
                }
            }
        );
    }

    ngAfterViewInit() {
        if (localStorage.getItem('isEdited') == 'yes') {
            this.toastr.toastrConfig.closeButton = true;
            this.toastr.success('Image has been edited successfully!', 'Success!', { closeButton: true });
            localStorage.removeItem('isEdited');
        } else if (localStorage.getItem('isAdded') == 'yes') {
            this.toastr.success('Image has been added successfully!', 'Success!', { closeButton: true});
            localStorage.removeItem('isAdded');
        }
        setTimeout(() => {
            this.pagination.page = localStorage.getItem('album-page') ? parseInt(localStorage.getItem('album-page')) : 1;
            this.changeDetectorRef.detectChanges();
        }, 1000);
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
            this.getAlbumIndexViewModel();
          },
          () => this.toastr.error('Access is denied!', 'Oops!', { closeButton: true}),
          () => {
            this.modalRef.hide();
            this.spinnerService.hide();
          }
        );
    }

    private getAlbumIndexViewModel(event?) {
        this.spinnerService.show();

        if (typeof event == 'string') {
            this.pagination.limit = parseInt(event);
            this.pagination.page = 1;
            localStorage.setItem('album-limit', this.pagination.limit.toString());
            localStorage.setItem('album-page', this.pagination.page.toString());
        } else if (typeof event == 'object') {
            this.pagination.limit = event.itemsPerPage;
            this.pagination.page = event.page;
            localStorage.setItem('album-limit', this.pagination.limit.toString());
            localStorage.setItem('album-page', this.pagination.page.toString());
        }
        setTimeout(() => {
            this.pagination.page = localStorage.getItem('album-page') ? parseInt(localStorage.getItem('album-page')) : 1;
            this.changeDetectorRef.detectChanges();
        }, 1000);

        this.galleryService.getAlbumIndexViewModel(this.pagination.limit, this.pagination.page)
        .then((response: any) => {
            this.albumIndexViewModel = response.images;
            this.pagination.totalItems = response.totalCount;
            console.log(response);
            this.scrollToTop();
            this.spinnerService.hide();
        }).catch(err => {
            this.toastr.error('Access is denied!', 'Oops!', { closeButton: true});
            this.spinnerService.hide();
        });
    }

    private scrollToTop() {
        window.scrollTo(0, 0);
    }

    goToAlbumView(album) {
        localStorage.setItem('album-title', album.title);
        this.router.navigate(['album-view', album.id]);
    }
}
