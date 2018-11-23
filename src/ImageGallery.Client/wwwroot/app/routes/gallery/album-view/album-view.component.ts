import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from "@angular/router";

import 'rxjs/add/operator/first';
import 'rxjs/add/operator/toPromise';

import { GalleryService } from '../../../gallery.service';
import { IGalleryIndexViewModel } from '../../../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { take } from 'rxjs/operators';

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

    constructor(private readonly galleryService: GalleryService,
        private activatedRoute: ActivatedRoute,
        public toastr: ToastrService, 
        private spinnerService: NgxLoadingSpinnerService,
        private changeDetectorRef: ChangeDetectorRef) {
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
      }).catch(err => {
          this.toastr.error('Access is denied!', 'Oops!', { closeButton: true});
          this.spinnerService.hide();
      });
    }
}
