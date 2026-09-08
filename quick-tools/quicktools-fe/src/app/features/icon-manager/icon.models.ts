import { PaginationRequest, PaginationResult } from '../../models/pagination.model';

export enum IconType {
    Gift,
    Image,
}

export interface IconModel {
    id: string;
    name: string;
    url: string;
    iconType: IconType;
}

export interface IconDTO {}

export interface SearchIconRequest extends PaginationRequest {}

export interface SearchIconResponse extends PaginationResult<IconModel> {}

export interface AddIconRequest {
    name: string;
    url: string;
    iconType: IconType,
}

export interface DeleteIconRequest {
    id: string;
}

export interface UpdateIconRequest {
    id: string;
    name: string;
    url: string;
    eIconModel: IconType;
}
