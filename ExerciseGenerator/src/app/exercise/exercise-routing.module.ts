import { ShowComponent } from './show/show.component';
import { EditComponent } from './edit/edit.component';
import { DeleteComponent } from './delete/delete.component';
import { CreateComponent } from './create/create.component';
import { ListComponent } from './list/list.component';

export const exerciseRoutes = [
  { path: '', component: ListComponent },
  { path: 'create', component: CreateComponent },
  { path: ':id', component: ShowComponent },
  { path: ':id/edit', component: EditComponent },
  { path: ':id/delete', component: DeleteComponent },
];
