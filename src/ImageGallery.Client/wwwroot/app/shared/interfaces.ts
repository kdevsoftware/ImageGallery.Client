export interface IImage {
    id: string;
    title: string;
    fileName: string;
    category: string;
}

export interface IRouteTypeModel {
    type: string;
}

export interface IGalleryIndexViewModel {
    images: IImage[];
    imagesUri: string;
}

export interface IEditImageViewModel {
    id: string;
    title: string;
    category: string;
}

export interface IAddImageViewModel {
    title: string;
    category: string;
    file: File;
}

export interface IImageViewModel {
    id: string;
    title: string;
    fileName: string;
}
