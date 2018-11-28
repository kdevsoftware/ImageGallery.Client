export interface IImage {
  id: string;
  title: string;
  fileName: string;
  category: string;
  dataSource: string;
  photoId: string;
  isPrimaryImage: boolean;
}

export interface IAlbum {
  id: string;
  title: string;
  descritpion: string;
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
  country: string;
}