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
import { AlbumSortComponent } from './album-view/album-sort/album-sort.component';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { TagInputModule } from 'ngx-chips';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ImageDetailsComponent } from '../../shared/image/details/image-details.component';
import { ImageMapComponent } from '../../shared/image/map/image-map.component';
import { ImageJsonComponent } from '../../shared/image/json/image-json.component';
import { AceEditorModule } from 'ng2-ace-editor';

@NgModule({
    imports: [
        SharedModule,
        PaginationModule.forRoot(),
        NgxLoadingSpinnerModule.forRoot(),
        ImageCropperModule,
        DragDropModule,
        BrowserAnimationsModule,
        TagInputModule,
        AceEditorModule
    ],
  declarations: [AlbumComponent, GalleryComponent, GalleryEditComponent, AlbumViewComponent, AlbumSortComponent, ImageDetailsComponent, ImageMapComponent, ImageJsonComponent, GalleryAddComponent, AboutComponent, KeysPipe],
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
        AlbumSortComponent,
        ImageDetailsComponent,
        ImageMapComponent,
        ImageJsonComponent
    ]
})
export class GalleryModule { }
