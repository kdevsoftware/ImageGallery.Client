import { Component, OnInit } from '@angular/core';

import { GalleryService } from '../../../gallery.service';
import { IAddImageViewModel } from '../../../shared/interfaces';
import { HasPayingUserRoleAuthenticationGuard } from '../../../guards/hasPayingUserRoleAuthenticationGuard';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { StorageService } from '../../../services/storage.service';
import { TitleService } from '../../../services/title.service';


@Component({
  selector: 'app-gallery-add',
  templateUrl: './gallery-add.component.html',
  styleUrls: ['./gallery-add.component.scss'],
  providers: [GalleryService, HasPayingUserRoleAuthenticationGuard]
})
export class GalleryAddComponent implements OnInit {

  addImageViewModel: IAddImageViewModel = { title: "", category: "Portraits", file: null };
  fileName: string;

  categories: string[] = ['Landscapes', 'Portraits', 'Animals'];

  constructor(
    private readonly galleryService: GalleryService,
    private router: Router,
    private toastr: ToastrService,
    private strage: StorageService,
    private titleService: TitleService) {
    this.fileName = 'Choose File';
  }

  ngOnInit() {
    this.titleService.set('Gallery Add Image');
  }

  onUpload(event: EventTarget) {
    let eventObj: MSInputMethodContext = <MSInputMethodContext>event;
    let target: HTMLInputElement = <HTMLInputElement>eventObj.target;
    let files: FileList = target.files;
    this.addImageViewModel.file = files[0];
    this.fileName = this.addImageViewModel.file.name;
  }

  public onSubmit() {
    console.log(`[onSubmit] app-gallery-add`);

    this.galleryService.postImageViewModel(this.addImageViewModel)
      .subscribe(() => { },
        (err: any) => {
          console.log(err);
          this.toastr.error('Failed to edit image!', 'Oops!', { closeButton: true });
        },
        () => {
          console.log('postImageViewModel() posted AddImageViewModel');

          this.strage.set('isAdded', 'yes');

          this.router.navigateByUrl("");
        });
  }
}
