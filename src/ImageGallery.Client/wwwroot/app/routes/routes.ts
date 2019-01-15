import { LayoutComponent } from '../layout/layout.component';

import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { RecoverComponent } from './pages/recover/recover.component';

import { GalleryComponent } from './gallery/gallery/gallery.component';
import { AlbumComponent } from './gallery/album/album.component';
import { GalleryEditComponent } from './gallery/gallery-edit/gallery-edit.component';
import { AlbumViewComponent } from './gallery/album-view/album-view.component';
import { GalleryModule } from './gallery/gallery.module';
import { GalleryAddComponent } from './gallery/gallery-add/gallery-add.component';
import { AboutComponent } from './gallery/about/about.component';

//Guards
import { HasPayingUserRoleAuthenticationGuard } from '../guards/hasPayingUserRoleAuthenticationGuard';
import { AuthGuard } from '../guards/authGuard';

export const routes = [

  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: GalleryComponent },
      { path: 'gallery', component: GalleryComponent },
      { path: 'albums', component: AlbumComponent },
      { path: 'gallery-add', component: GalleryAddComponent, canActivate: [HasPayingUserRoleAuthenticationGuard] },
      { path: 'album-view/:id', component: AlbumViewComponent },
      { path: 'gallery-edit/:id', component: GalleryEditComponent },
      { path: 'about', component: AboutComponent }
    ]
    , canActivate: [AuthGuard]
  },

  // Not lazy-loaded routes
  {
    path: 'login'
    , children: [
      { path: '', component: LoginComponent }
      , { path: 'recover', component: RecoverComponent }
      , { path: 'register', component: RegisterComponent }
    ]
  },
  // Not found
  { path: '**', redirectTo: 'login' }

];
