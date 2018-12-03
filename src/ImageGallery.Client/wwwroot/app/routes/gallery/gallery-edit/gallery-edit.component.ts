import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from "@angular/router";

import 'rxjs/add/operator/first';
import 'rxjs/add/operator/toPromise';

import { GalleryService } from '../../../gallery.service';
import { IEditImageViewModel } from '../../../shared/interfaces';
import { ToastrService } from 'ngx-toastr';
import { ImageCroppedEvent } from 'ngx-image-cropper/src/image-cropper.component';
import { NgxLoadingSpinnerService } from 'ngx-loading-spinner-fork';
import { StorageService } from '../../../services/storage.service';
import { TitleService } from '../../../services/title.service';

@Component({
  selector: 'app-gallery-edit',
  templateUrl: './gallery-edit.component.html',
  styleUrls: ['./gallery-edit.component.scss'],
  providers: [GalleryService]
})
export class GalleryEditComponent implements OnInit {
  @ViewChild('image') image;
  imageUrl;
  imageBase64;

  editImageViewModel: IEditImageViewModel;

  categories: string[] = ['Landscapes', 'Portraits', 'Animals'];

  croppedImage: any = '';
  croppedImageFile: any = '';

  constructor(private readonly galleryService: GalleryService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    public toastr: ToastrService,
    private spinnerService: NgxLoadingSpinnerService,
    public storage: StorageService,
    private titleService: TitleService) { }

  async ngOnInit() {
    this.titleService.set('Gallery Edit Image');

    const imageId = await this.getImageIdAsync();

    console.log(`Image id: ${imageId}`);

    this.getEditImageViewModel(imageId);
  }

  public onSubmit() {
    console.log(`[onSubmit] app-gallery-edit`);

    this.galleryService.postEditImageViewModel(this.editImageViewModel)
      .subscribe(() => { },
        (err: any) => {
          this.toastr.error('Failed to edit image!', 'Oops!', { closeButton: true });
          console.log(err);
        },
        () => {
          console.log('postEditImageViewModel() posted EditImageViewModel');
          this.storage.set('isEdited', 'yes');
          this.router.navigate(['/']);
        });
  }

  imageCropped(event: ImageCroppedEvent) {
    if (event.file) {
      this.croppedImage = event.base64;
      this.croppedImageFile = event.file;
    }
  }

  imageLoaded() { }

  loadImageFailed() { }

  imageLoad() {
    this.imageBase64 = this.getBase64Image(this.image.nativeElement);
  }

  cropImage() {
    this.spinnerService.show();

    this.galleryService.cropImage(this.editImageViewModel.id, this.croppedImageFile)
      .subscribe(
        () => {
          this.toastr.success('Image updated successfully!', 'Success!', { closeButton: true });
          this.spinnerService.hide();
        },
        () => {
          this.toastr.error('Access is denied!', 'Oops!', { closeButton: true });
          this.spinnerService.hide();
        }
      );
  }

  private async getImageIdAsync(): Promise<string> {
    const params = await this.activatedRoute.paramMap.first().toPromise();
    const imageId = params.get('id');
    return imageId;
  }

  private getEditImageViewModel(imageId: string) {
    this.spinnerService.show();

    this.galleryService.getEditImageViewModel(imageId)
      .subscribe((response: IEditImageViewModel) => {
        this.editImageViewModel = response;
        // todo: for test
        this.editImageViewModel.imageUrl = '../../../../assets/img/test.jpg';
        this.spinnerService.hide();
      },
        (err: any) => {
          console.log(err);
          this.spinnerService.hide();
        },
        () => console.log('getEditImageViewModel() retrieved EditImageViewModel'));
  }

  private getBase64Image(img) {
    let canvas = document.createElement('canvas');
    let context = canvas.getContext('2d');

    canvas.width = img.width;
    canvas.height = img.height;

    context.drawImage(img, 0, 0);
    canvas.toDataURL('image/png');

    return canvas.toDataURL('image/png');
  }
}
