import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { UserDto, CreateUserDto, UserListDto } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiService = inject(ApiService);

  getAll(includeDeleted: boolean = false): Observable<UserListDto[]> {
    // Pass includeDeleted as query string when requested
    const suffix = includeDeleted ? '?includeDeleted=true' : '';
    return this.apiService.get<UserListDto[]>(`User${suffix}`);
  }

  getById(id: number): Observable<UserDto> {
    return this.apiService.get<UserDto>(`User/${id}`);
  }

  create(user: CreateUserDto): Observable<UserDto> {
    // Map the frontend CreateUserDto to the backend UserDto shape expected by the API.
    // Note: backend UserDto contains FullName and PersonId; we map `username` -> `FullName`.
    const payload: any = {
      fullName: (user as any).username ?? '',
      email: user.email,
      password: user.password,
      rolId: user.rolId,
      personId: (user as any).personId ?? 0 // if you can provide an existing personId, set it here
    };
    return this.apiService.post<UserDto>('User', payload);
  }

  // Backend expects PUT /api/User/{id} to update a user (see BaseController)
  update(user: UserDto): Observable<UserDto> {
    if (!user.id) throw new Error('User id is required to update');
    return this.apiService.put<UserDto>(`User/${user.id}`, user);
  }

  delete(id: number): Observable<void> {
    // Backend provides logical delete at DELETE /api/User/logic/{id}
    return this.apiService.delete<void>(`User/logic/${id}`);
  }

  // Reactivate (logical) a user by setting active = true via PATCH /api/User/{id}
  activate(id: number): Observable<void> {
    return this.apiService.patch<void>(`User/${id}`, { active: true });
  }
}
