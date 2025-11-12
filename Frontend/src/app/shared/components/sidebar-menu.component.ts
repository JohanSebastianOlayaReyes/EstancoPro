import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { IconComponent } from './icon.component';

export interface MenuItem {
  title: string;
  icon: string;
  route: string;
}

export interface MenuSection {
  title: string;
  items: MenuItem[];
}

@Component({
  selector: 'app-sidebar-menu',
  standalone: true,
  imports: [CommonModule, RouterLink, IconComponent],
  template: `
    <nav class="left-nav" style="background:#1b7d3f;">
      <div class="nav-brand" style="color:#ffffff; border-bottom-color: rgba(255,255,255,0.2);">
        <app-icon name="settings"></app-icon> Admin
      </div>

      <div class="nav-section" *ngFor="let section of sections">
        <div class="nav-heading" style="color: rgba(255,255,255,0.7);">{{ section.title }}</div>
        <ul>
          <li *ngFor="let item of section.items">
            <a class="nav-link" [routerLink]="item.route" style="color:#ffffff;">
              <app-icon [name]="item.icon" [size]="16"></app-icon>
              <span class="nav-title">{{ item.title }}</span>
            </a>
          </li>
        </ul>
      </div>
    </nav>
  `,
  styles: [`
    :host {
      display: block;
    }

    .left-nav {
      width: 250px;
      height: 100vh;
      overflow-y: auto;
      padding: 20px 0;
      position: fixed;
      left: 0;
      top: 0;
      z-index: 100;
      box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
    }

    .nav-brand {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 15px 20px;
      border-bottom: 1px solid;
      font-weight: bold;
      font-size: 16px;
      margin-bottom: 20px;
    }

    .nav-section {
      margin-bottom: 30px;
    }

    .nav-heading {
      padding: 10px 20px;
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .nav-section ul {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .nav-section li {
      margin: 0;
    }

    .nav-link {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 20px;
      text-decoration: none;
      transition: background-color 0.2s ease;
      cursor: pointer;

      &:hover {
        background-color: rgba(255, 255, 255, 0.1);
      }

      &.router-link-active {
        background-color: rgba(255, 255, 255, 0.2);
        border-left: 3px solid #fff;
        padding-left: 17px;
      }
    }

    .nav-title {
      font-size: 14px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  `]
})
export class SidebarMenuComponent {
  @Input() sections: MenuSection[] = [];
}
