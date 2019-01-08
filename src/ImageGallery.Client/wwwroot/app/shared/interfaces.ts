export interface IImage {
  id: string;
  title: string;
  fileName: string;
  category: string;
  sort: number;
  dataSource: string;
  width: number;
  height: number;
  photoId: string;
  isPrimaryImage: boolean;
}

export interface IAlbum {
  id: string;
  title: string;
  description: string;
  dateCreated: Date;
}

export interface IAlbumTag {
  id: string;
  tag: string;
}

export interface IAlbumMetadata extends IAlbum {
  albumTags: IAlbumTag[];
}

export interface IRouteTypeModel {
  type: string;
}

export interface IAlbumIndexViewModel {
  albums: IAlbum[];
  albumsUri: string;
}

export interface IGalleryIndexViewModel {
  images: IImage[];
  imagesUri: string;
}

export interface IEditImageViewModel {
  id: string;
  title: string;
  category: string;
  imageUrl: string;
  height: number;
  width: number;
}

export interface IAddImageViewModel {
  title: string;
  category: string;
  file: File;
}

export interface IAlbumViewModel {
  id: string;
  title: string;
  description: string;
}

export interface IUserProfileViewModel {
  firstName: string;
  lastName: string;
  address: string;
  address2: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  language: string;
}
