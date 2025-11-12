import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { RolDto } from '../models/rol.model';

@Injectable({
  providedIn: 'root'
})
export class RolService {
  private apiService = inject(ApiService);

  getAll(): Observable<RolDto[]> {
    return this.apiService.get<RolDto[]>('Rol');
  }

  getById(id: number): Observable<RolDto> {
    return this.apiService.get<RolDto>(`Rol/${id}`);
  }

  create(rol: RolDto): Observable<RolDto> {
    return this.apiService.post<RolDto>('Rol', rol);
  }

  update(rol: RolDto): Observable<RolDto> {
    return this.apiService.put<RolDto>('Rol', rol);
  }

  delete(id: number): Observable<void> {
    return this.apiService.delete<void>(`Rol/${id}`);
  }
}
