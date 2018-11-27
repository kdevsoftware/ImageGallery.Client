import { NgModule } from '@angular/core';
import { GalleryComponent } from './gallery/gallery.component';
import { AlbumComponent } from './album/album.component';
import { GalleryEditComponent } from './gallery-edit/gallery-edit.component';
import { AlbumViewComponent } from './album-view/album-view.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { GalleryAddComponent } from './gallery-add/gallery-add.component';
import { AboutComponent } from './about/about.component';
import { KeysPipe } from '../../pipes/keys.pipe';
import { PaginationModule } from 'ngx-bootstrap';
import { NgxLoadingSpinnerModule } from 'ngx-loading-spinner-fork';
import { ImageCropperModule } from 'ngx-image-cropper';

@NgModule({
    imports: [
        SharedModule,
        PaginationModule.forRoot(),
        NgxLoadingSpinnerModule.forRoot(),
        ImageCropperModule
    ],
    declarations: [AlbumComponent, GalleryComponent, GalleryEditComponent, AlbumViewComponent, GalleryAddComponent, AboutComponent, KeysPipe],
    exports: [
        RouterModule,
        GalleryComponent,
        AlbumComponent,
        GalleryEditComponent,
        AlbumViewComponent,
        GalleryAddComponent,
        AboutComponent,
        PaginationModule,
        NgxLoadingSpinnerModule,
    ]
})
export class GalleryModule { }
