using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrexRunner.Entities
{
    public class EntityManager
    {
        
        private readonly List<IGameEntity> _entities = new List<IGameEntity>();

        private readonly List<IGameEntity> _entitiesToAdd = new List<IGameEntity>();
        private readonly List<IGameEntity> _entitiesToRemove = new List<IGameEntity>();

        //Allows "client" to create an enumerable read only list known as "Entities" to iterate over the members of _entities
        //Create an object of type EntityManager *name* = new EntityManager(); in other classes
        //Allows (foreach *name of variable* in *name of EntityManager*)
        //{do some stuff;}
        public IEnumerable<IGameEntity> Entities => new ReadOnlyCollection<IGameEntity>(_entities);

        public void Update(GameTime gameTime)
        {

            //Updates entities in the entity list
            foreach(IGameEntity entity in _entities)
            {
                //skips updating entities in entities to remove 
                if(_entitiesToRemove.Contains(entity))
                    continue;
                
                entity.Update(gameTime);

            }

            //Adds each entity in the pending add list to the entity list
            foreach(IGameEntity entity in _entitiesToAdd)
            {

                _entities.Add(entity);

            }

            //removes each entity in the pending remove list from the entity list
            foreach (IGameEntity entity in _entitiesToRemove)
            {

                _entities.Remove(entity);

            }
            
            //Clears the following lists as to not act more than once
            _entitiesToAdd.Clear();
            _entitiesToRemove.Clear();
        }

        //Draws each entity in the order of the DrawOrder variable
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            foreach(IGameEntity entity in _entities.OrderBy(e => e.DrawOrder))
            {

                entity.Draw(spriteBatch, gameTime);

            }

        }

        //Adds the passed entity to a list to be added later as to not add entities to a currently updating list
        public void AddEntity(IGameEntity entity)
        {

            if(entity == null)
                throw new ArgumentNullException(nameof(entity), "Null cannot be added as an entity");

                _entitiesToAdd.Add(entity);

        }

        //Adds the passed entity to a list to be removed later as to not add entities to a currently updating list
        public void RemoveEntity(IGameEntity entity)
        {

            if(entity == null)
                throw new ArgumentNullException(nameof(entity), "Null is not a valid entity");

            _entitiesToRemove.Add(entity);

        }

        public void Clear()
        {

            _entitiesToRemove.AddRange(_entities);

        }

        //method which returns all entities of the specified type
        public IEnumerable<T> GetEntitiesOfType<T>() where T : IGameEntity
        {

            return _entities.OfType<T>();

        }

    }
}