import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Injectable()
export class TitleService {
  constructor(private title: Title) { }

  set(title?: string) {
    const newTitle = title ? title + ' - Image Gallery' : 'Image Gallery';
    this.title.setTitle(newTitle);
  }
}