﻿import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Rx'
import 'rxjs/add/operator/catch';

import { IEditImageViewModel, IAddImageViewModel, IGalleryIndexViewModel, IAlbumMetadata } from './shared/interfaces';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { OAuthService } from 'angular-oauth2-oidc';

@Injectable()
export class GalleryService {

  private baseUrl: string = '/api/images';
  private albumUrl: string = '/api/albums';
  private photoUrl = '';
  private reportsUrl = 'https://api-reports.navigatorglass.com/api/Reference';

  constructor(private httpClient: HttpClient, private oauthService: OAuthService) {
    this.getConfig().subscribe((res: any) => {
      this.photoUrl = res.clientConfiguration.apiAttractionsUri;
    });
  }

  private getConfig() {
    return this.httpClient.get('api/ClientAppSettings');
  }

  public getGalleryIndexViewModel(limit: number, page: number, searchInput?: string) {
    var self = this;
    var searchQuery = '';

    if (searchInput) {
      searchQuery = `Search=${searchInput}&`;
    }

    var query = `${this.baseUrl}/list?${searchQuery}limit=${limit}&page=${page}`;

    return new Promise((resolve, reject) => {
      this.httpClient.get(query, { observe: 'response', headers: self.generateBearerHeaaders() })
        .subscribe(res => {
          resolve({
            totalCount: res.headers.get('X-InlineCount'),
            images: res.body
          });
        }, error => {
          reject(error);
        });
    });
  }

  public getAlbumIndexViewModel(limit: number, page: number) {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-type", "application/json");
    return new Promise((resolve, reject) => {
      this.httpClient.get(`${this.albumUrl}/list?limit=${limit}&page=${page}`, { observe: 'response', headers: headers })
        .subscribe(res => {
          resolve({
            totalCount: res.headers.get('X-InlineCount'),
            images: res.body
          });
        }, error => {
          reject(error);
        });
    });
  }

  public getAlbumViewModel(id: string, limit: number, page: number) {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-Type", "application/json");

    var query = '';

    if (limit && page) {
      query = `${this.albumUrl}/images/list/${limit}/${page}?id=${id}`;
    } else {
      query = `${this.albumUrl}/images/list/?id=${id}`;
    }

    return new Promise((resolve, reject) => {
      this.httpClient.get(query, { observe: 'response', headers: headers })
        .subscribe(res => {
          resolve({
            totalCount: res.headers.get('X-InlineCount'),
            images: res.body
          });
        }, error => {
          reject(error);
        });
    });
  }

  public getAlbumMetadata(id: string): Observable<IAlbumMetadata> {
    var self = this;
    return this.httpClient.get<IAlbumMetadata>(`${this.albumUrl}/metadata?id=${id}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public postAlbumMetadataTag(id: string, tag: string): Observable<Object> {
    var self = this;
    return this.httpClient.post(`${this.albumUrl}/metadata/tags?id=${id}`, {tag}, { headers: self.generateBearerHeaaders() });
  }

  public deleteAlbumMetadataTag(albumId: string, tagId: string) {
    var self = this;
    return this.httpClient.delete(`${this.albumUrl}/metadata/tags/${albumId}?tagId=${tagId}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public getEditImageViewModel(id: string): Observable<IEditImageViewModel> {
    var self = this;
    return this.httpClient.get<IEditImageViewModel>(`${this.baseUrl}/${id}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public getPhotoAttraction(id: string): Observable<Object> {
    return this.httpClient.get(`${this.photoUrl}/api/photo/attraction/${id}`, { headers: this.generateBearerHeaaders() }).catch(this.handleError);
  }

  public postEditImageViewModel(model: IEditImageViewModel): Observable<Object> {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-Type", "application/json");

    return this.httpClient.post(`${this.baseUrl}/edit`, model, { headers: headers });
  }

  public patchAlbumProperty(id: string, propertyName: string, propertyValue: string): Observable<Object> {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-Type", "application/json-patch+json");

    const model = [
      {
        propertyName: propertyName,
        propertyValue: propertyValue
      }
    ];

    return this.httpClient.patch(`${this.albumUrl}/${id}`, model, { headers: headers });
  }

  public patchImageTitle(id: string, propertyName: string, propertyValue: string): Observable<Object> {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-Type", "application/json-patch+json");

    const model = [
      {
        propertyName: propertyName,
        propertyValue: propertyValue
      }
    ];

    return this.httpClient.patch(`${this.baseUrl}/${id}`, model, { headers: headers });
  }

  public deleteImageViewModel(id: string): Observable<Object> {
    var self = this;
    return this.httpClient.delete(`${this.baseUrl}/${id}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public deleteImageFromAlbum(id: string, imageId: string): Observable<Object> {
    var self = this;
    return this.httpClient.delete(`${this.albumUrl}/${id}/${imageId}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public deleteAlbumViewModel(id: string): Observable<Object> {
    var self = this;
    return this.httpClient.delete(`${this.albumUrl}/${id}`, { headers: self.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public cropImage(id: string, file: File): Observable<Object> {
    const formData = new FormData();
    formData.append('Id', id);
    formData.append('File', file);

    var options = { headers: this.generateBearerHeaaders() }

    return this.httpClient.post(`${this.baseUrl}/update`, formData, options)
      .catch(this.handleError);
  }

  public postImageViewModel(model: IAddImageViewModel): Observable<Object> {
    let formData = new FormData();
    formData.append('Title', model.title);
    formData.append('Category', model.category);
    formData.append('File', model.file);

    var options = { headers: this.generateBearerHeaaders() }

    return this.httpClient.post(`${this.baseUrl}/add`, formData, options)
      .catch(this.handleError);
  }

  public putAlbumImagesSort(id: string, model: any) {
    var self = this;
    return this.httpClient.put(`${this.albumUrl}/${id}/sort`, model, { headers: self.generateBearerHeaaders() });
  }

  public setPrimaryAlbumImage(id: string, imageId: string): Observable<Object> {
    var headers = this.generateBearerHeaaders();
    headers.append("Content-Type", "application/json");

    return this.httpClient.put(`${this.albumUrl}/primaryimage/${id}?imageId=${imageId}`, {}, { headers: headers });
  }

  public getReports(reportType: string) {
    return this.httpClient.get(`${this.reportsUrl}/${reportType}`, { headers: this.generateBearerHeaaders() })
      .catch(this.handleError);
  }

  public getImageBase64(id: string) {
    var self = this;
    return this.httpClient.get(`${this.baseUrl}/text/${id}`, { headers: self.generateBearerHeaaders(), responseType: 'text' })
      .catch(this.handleError);
  }

  private generateBearerHeaaders(): HttpHeaders {
    return new HttpHeaders({
      "Authorization": "Bearer " + this.oauthService.getAccessToken()
    });
  }

  private handleError(error: any) {
    console.error('server error:', error);
    if (error instanceof HttpErrorResponse) {
      let errMessage = '';
      try {
        errMessage = error.error;
      } catch (err) {
        errMessage = error.statusText;
      }
      return Observable.throw(errMessage);
      // Use the following instead if using lite-server
      //return Observable.throw(err.text() || 'backend server error');
    }
    return Observable.throw(error || 'ASP.NET Core server error');
  }
}
